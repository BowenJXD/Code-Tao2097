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
    /// <summary>
    /// 各种数值修改方式的枚举
    /// 最终的数值计算方式为：(基础值 + 基础值修改的和) * 乘法修改的积 * (1 + 加乘修改的和) + 加法修改的和
    /// </summary>
    public enum EModifierType
    {
        Basic,
        Additive,
        MultiAdd,
        Multiplicative
    }

    /// <summary>
    /// 对应一个数值修改群组，可以绑定事件，在数值修改群组发生变化时，触发绑定的事件
    /// 可以注册多个数值修改群组
    /// </summary>
    [Serializable]
    public class BindableStat : BindableProperty<float>
    {
        private float _minValue = float.MinValue;
        private float _maxValue = float.MaxValue;
        [SerializeField] protected float valueCache;
        [SerializeField] protected string str;

        protected override float GetValue()
        {
            if (valueCache == default)
            {
                valueCache = CalculateValue();
            }
            return valueCache;
        }

        protected override void SetValue(float newValue)
        {
            AddModifier(newValue - mValue, EModifierType.Basic, "self", RepetitionBehavior.Overwrite);
        }

        protected override void Change()
        {
            valueCache = CalculateValue();
            str = ToString();
            base.Change();
        }

        private ModifierGroup _mainModGroup;
        private List<ModifierGroup> _modGroups = new List<ModifierGroup>();

        public List<ModifierGroup> ModGroups
        {
            get => _modGroups;
            set => _modGroups = value;
        }

        public BindableStat() : base(0)
        {
            Init();
        }

        public BindableStat(float value) : base(value)
        {
            Init();
        }

        public void Init()
        {
            valueCache = CalculateValue();
            _mainModGroup = new ModifierGroup(Change);
            _modGroups.Add(_mainModGroup);
        }
        
        public BindableStat SetMinValue(float value, bool clamp = false)
        {
            _minValue = value;
            if (clamp && Value < _minValue)
            {
                SetValue(_minValue);
            }
            return this;
        }
        
        public BindableStat SetMaxValue(float value, bool clamp = true)
        {
            _maxValue = value;
            if (clamp && Value > _maxValue)
            {
                SetValue(_maxValue);
            }
            return this;
        }
        
        public bool AddModifier(float value, EModifierType modifierType, string name = "",
            RepetitionBehavior repetitionBehavior = RepetitionBehavior.Return, bool times = false)
        {
            if (times && modifierType is EModifierType.MultiAdd or EModifierType.Multiplicative)
            {
                value -= 1;
            }
            bool result = _mainModGroup.AddModifier(value, modifierType, name, repetitionBehavior);
            if (result)
            {
                Change();
            }

            return result;
        }

        public bool RemoveModifier(EModifierType modifierType, string name = "")
        {
            bool result = false;
            result = _mainModGroup.RemoveModifier(modifierType, name);

            if (result)
            {
                Change();
            }

            return result;
        }
        
        public bool AddModifierGroup(ModifierGroup modifierGroup, bool notifyChange = true)
        {
            bool result = false;
            if (!_modGroups.Contains(modifierGroup))
            {
                _modGroups.Add(modifierGroup);
                modifierGroup.onChanged += () => { Change(); };
                result = true;
                if (notifyChange)
                {
                    Change();
                }
            }

            return result;
        }
        
        public bool RemoveModifierGroup(ModifierGroup modifierGroup)
        {
            bool result = false;
            if (_modGroups.Contains(modifierGroup))
            {
                _modGroups.Remove(modifierGroup);
                modifierGroup.onChanged -= () => { Change(); };
                result = true;
            }

            return result;
        }
        
        public void AddModifierGroups(List<ModifierGroup> modifierGroups, bool notifyChange = true)
        {
            foreach (var modifierGroup in modifierGroups)
            {
                AddModifierGroup(modifierGroup, false);
            }
            if (notifyChange)
            {
                Change();
            }
        }
        
        public void RemoveModifierGroups(List<ModifierGroup> modifierGroups)
        {
            foreach (var modifierGroup in modifierGroups)
            {
                RemoveModifierGroup(modifierGroup);
            }
        }

        protected float CalculateValue()
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
            valueCache = resultValue;
            return resultValue;
        }
        
        public void Reset()
        {
            _mainModGroup.Clear(false);
            _modGroups.Clear();
            _modGroups.Add(_mainModGroup);
            Change();
            mOnValueChanged = null;
        }
        
        public void InheritStat(BindableStat otherStat)
        {
            // if negative, inherit the base value from otherStat
            if (mValue < 0)
            {
                SetValueWithoutEvent(-mValue * otherStat);
                ModGroups = otherStat.ModGroups;
            }
            // if positive, only inherit the modifier groups
            else
            {
                AddModifierGroups(otherStat.ModGroups);
            }
        }
        
        public static implicit operator float(BindableStat myObject)
        {
            return myObject.Value;
        }
        
        // to string
        public override string ToString()
        {
            string result = "";
            foreach (var modifierGroup in _modGroups)
            {
                result += modifierGroup.ToString() + "\n";
            }

            return result;
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
            EditorGUI.BeginProperty(position, label, property);

            // Calculate the width for each property field
            float singleFieldWidth = 3 * position.width / 4f;

            // Split the position into two halves
            Rect mValueRect = new Rect(position.x, position.y, singleFieldWidth, position.height);
            Rect readOnlyValueRect = new Rect(position.x + singleFieldWidth, position.y, position.width / 4f, position.height);

            // Find the SerializedProperty for mValue and readOnlyValue
            SerializedProperty mValueProperty = property.FindPropertyRelative("mValue");
            SerializedProperty readOnlyValueProperty = property.FindPropertyRelative("valueCache");
            SerializedProperty stringValueProperty = property.FindPropertyRelative("str");

            // Display the mValue property field
            EditorGUI.PropertyField(mValueRect, mValueProperty, label);

            // Display a readonly label for readOnlyValue
            EditorGUI.LabelField(readOnlyValueRect, readOnlyValueProperty.floatValue.ToString("F2"), EditorStyles.label);

            if (readOnlyValueRect.Contains(Event.current.mousePosition))
            {
                // Check if the mouse was clicked
                if (Event.current.type == EventType.MouseDown)
                {
                    // Log the ToString value when clicking
                    Debug.Log($"{label}: {stringValueProperty.stringValue}");
                }
            }

            EditorGUI.EndProperty();
        }
    }
}

