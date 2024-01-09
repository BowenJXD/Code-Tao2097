using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using QFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public enum EModifierType
    {
        Basic,
        Additive,
        MultiAdd,
        Multiplicative
    }

    public enum ERepetitionBehavior
    {
        Return,
        Overwrite,
        AddStack,
        NewStack
    }

    public class ModifierGroup
    {
        public string Source { get; set; }
        
        private Dictionary<EModifierType, Dictionary<string, float>> _modifiers = EModifierType.GetValues(typeof(EModifierType))
            .Cast<EModifierType>()
            .ToDictionary(key => key, value => new Dictionary<string, float>());

        public ModifierGroup(Action newOnChanged = null)
        {
            if (newOnChanged != null)
            {
                onChanged += newOnChanged;
            }
        }
        
        public Dictionary<string, float> GetModifier(EModifierType modifierType)
        {
            return _modifiers[modifierType];
        }

        public Action onChanged;
        
        public bool AddModifier(float value, EModifierType modifierType, string name = "", 
            ERepetitionBehavior repetitionBehavior = ERepetitionBehavior.Return)
        {
            bool result = false;
            Dictionary<string, float> modifiers = _modifiers[modifierType];
            if (modifiers == null)
            {
                return result;
            }
            if (name == "")
            {
                repetitionBehavior = ERepetitionBehavior.NewStack;
            }

            if (modifiers.ContainsKey(name))
            {
                switch (repetitionBehavior)
                {
                    case ERepetitionBehavior.Return:
                        break;
                    case ERepetitionBehavior.Overwrite:
                        modifiers[name] = value;
                        result = true;
                        break;
                    case ERepetitionBehavior.AddStack:
                        modifiers[name] += value;
                        result = true;
                        break;
                    case ERepetitionBehavior.NewStack: // not implemented
                        name += modifiers.Count;
                        modifiers.Add(name, value);
                        result = true;
                        break;
                }
            }
            else
            {
                result = modifiers.TryAdd(name, value);
            }

            if (result)
            {
                onChanged?.Invoke();
            }

            return result;
        }
        
        public bool RemoveModifier(EModifierType modifierType, string name = "")
        {
            bool result = false;
            if (name == "")
            {
                result = _modifiers[modifierType].Count > 0;
                _modifiers[modifierType].Clear();
            }
            else
            {
                result = _modifiers[modifierType].Remove(name);
            }
            return result;
        }
        
        /// <summary>
        ///  Clear all modifiers
        /// </summary>
        /// <param name="change"> Whether to trigger change or not, default to true </param>
        /// <returns></returns>
        public bool Clear(bool change = true)
        {
            if (_modifiers.Count > 0)
            {
                foreach (var keyValuePair in _modifiers)
                {
                    keyValuePair.Value.Clear();
                }
                if (change)
                {
                    onChanged?.Invoke();
                }
                return true;
            }

            return false;
        }
    }
    
    [Serializable]
    public class BindableStat : BindableProperty<float>
    {
        private float _initValue = 0;
        private float _minValue = float.MinValue;
        private float _maxValue = float.MaxValue;

        private ModifierGroup _mainModGroup;
        private List<ModifierGroup> _modGroups = new List<ModifierGroup>();

        public List<ModifierGroup> ModGroups
        {
            get => _modGroups;
        }

        public BindableStat()
        {
            _initValue = mValue;
            _mainModGroup = new ModifierGroup(() => mOnValueChanged?.Invoke(Value));
            _modGroups.Add(_mainModGroup);
        }

        public BindableStat(float value) : base(value)
        {
            _initValue = mValue;
            _mainModGroup = new ModifierGroup(() => mOnValueChanged?.Invoke(Value));
            _modGroups.Add(_mainModGroup);
        }
        
        public BindableStat SetMinValue(float value)
        {
            _minValue = value;
            return this;
        }
        
        public BindableStat SetMaxValue(float value)
        {
            _maxValue = value;
            return this;
        }

        public bool AddModifier(float value, EModifierType modifierType, string name = "", 
            ERepetitionBehavior repetitionBehavior = ERepetitionBehavior.Return)
        {
            bool result = _mainModGroup.AddModifier(value, modifierType, name, repetitionBehavior);
            if (result)
            {
                mOnValueChanged?.Invoke(Value);
            }

            return result;
        }

        public bool RemoveModifier(EModifierType modifierType, string name = "")
        {
            bool result = false;
            result = _mainModGroup.RemoveModifier(modifierType, name);

            if (result)
            {
                mOnValueChanged?.Invoke(Value);
            }

            return result;
        }
        
        public bool AddModifierGroup(ModifierGroup modifierGroup)
        {
            bool result = false;
            if (!_modGroups.Contains(modifierGroup))
            {
                _modGroups.Add(modifierGroup);
                modifierGroup.onChanged += () => { mOnValueChanged?.Invoke(Value); };
                result = true;
            }

            return result;
        }
        
        public bool RemoveModifierGroup(ModifierGroup modifierGroup)
        {
            bool result = false;
            if (_modGroups.Contains(modifierGroup))
            {
                _modGroups.Remove(modifierGroup);
                modifierGroup.onChanged -= () => { mOnValueChanged?.Invoke(Value); };
                result = true;
            }

            return result;
        }
        
        public void AddModifierGroups(List<ModifierGroup> modifierGroups)
        {
            foreach (var modifierGroup in modifierGroups)
            {
                AddModifierGroup(modifierGroup);
            }
        }
        
        public void RemoveModifierGroups(List<ModifierGroup> modifierGroups)
        {
            foreach (var modifierGroup in modifierGroups)
            {
                RemoveModifierGroup(modifierGroup);
            }
        }

        protected override float GetValue()
        {
            float basic = 0;
            float additive = 0;
            float multiAdd = 0;
            float multiplicative = 1;

            foreach (var modifierGroup in _modGroups)
            {
                basic += modifierGroup.GetModifier(EModifierType.Basic).Values.Sum();
                additive += modifierGroup.GetModifier(EModifierType.Additive).Values.Sum();
                multiAdd += modifierGroup.GetModifier(EModifierType.MultiAdd).Values.Sum();
                multiplicative *= modifierGroup.GetModifier(EModifierType.Multiplicative).Values.Aggregate(1f, (current, value) => current * (1 + value));
            }

            float resultValue = (mValue + basic) * multiplicative * (1 + multiAdd) + additive;
            resultValue = Math.Clamp(resultValue, _minValue, _maxValue);
            return resultValue;
        }
        
        public void Reset()
        {
            _mainModGroup.Clear(false);
            _modGroups.Clear();
            _modGroups.Add(_mainModGroup);
            mOnValueChanged = null;
        }
        
        public static implicit operator float(BindableStat myObject)
        {
            return myObject.Value;
        }
    }
    
    [CustomPropertyDrawer(typeof(BindableProperty<>))]
    public class BindablePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position,label,property);
            SerializedProperty mValueProperty = property.FindPropertyRelative("mValue");
            EditorGUI.PropertyField(position, mValueProperty, label);
            EditorGUI.EndProperty();
        }
    }
    
    [CustomPropertyDrawer(typeof(BindableStat))]
    public class BindableStatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position,label,property);
            SerializedProperty mValueProperty = property.FindPropertyRelative("mValue");
            EditorGUI.PropertyField(position, mValueProperty, label);
            EditorGUI.EndProperty();
        }
    }
}

