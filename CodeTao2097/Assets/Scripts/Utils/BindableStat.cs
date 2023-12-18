using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using QFramework;
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
        private Dictionary<EModifierType, Dictionary<string, float>> _modifiers = EModifierType.GetValues(typeof(EModifierType))
            .Cast<EModifierType>()
            .ToDictionary(key => key, value => new Dictionary<string, float>());

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
        
        public void Clear()
        {
            foreach (var keyValuePair in _modifiers)
            {
                keyValuePair.Value.Clear();
            }
        }
    }
    
    [Serializable]
    public class BindableStat : BindableProperty<float>
    {
        private float _initValue = 0;
        private float _minValue = 0;
        private float _maxValue = float.MaxValue;
        private ModifierGroup _modiferGroup = new ModifierGroup();
        
        private List<ModifierGroup> _otherModGroups = new List<ModifierGroup>();

        public BindableStat()
        {
            _initValue = mValue;
            _modiferGroup.onChanged += () => { mOnValueChanged?.Invoke(Value); };
        }
        
        public BindableStat(float value) : base(value)
        {
            _initValue = mValue;
            _modiferGroup.onChanged += () => { mOnValueChanged?.Invoke(Value); };
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
            bool result = _modiferGroup.AddModifier(value, modifierType, name, repetitionBehavior);
            if (result)
            {
                mOnValueChanged?.Invoke(Value);
            }

            return result;
        }

        public bool RemoveModifier(EModifierType modifierType, string name = "")
        {
            bool result = false;
            result = _modiferGroup.RemoveModifier(modifierType, name);

            if (result)
            {
                mOnValueChanged?.Invoke(Value);
            }

            return result;
        }
        
        public bool AddModifierGroup(ModifierGroup modifierGroup)
        {
            bool result = false;
            if (!_otherModGroups.Contains(modifierGroup))
            {
                _otherModGroups.Add(modifierGroup);
                modifierGroup.onChanged += () => { mOnValueChanged?.Invoke(Value); };
                result = true;
            }

            return result;
        }
        
        public bool RemoveModifierGroup(ModifierGroup modifierGroup)
        {
            bool result = false;
            if (_otherModGroups.Contains(modifierGroup))
            {
                _otherModGroups.Remove(modifierGroup);
                modifierGroup.onChanged -= () => { mOnValueChanged?.Invoke(Value); };
                result = true;
            }

            return result;
        }

        protected override float GetValue()
        {
           
            float basic = _modiferGroup.GetModifier(EModifierType.Basic).Values.Sum();
            float additive = _modiferGroup.GetModifier(EModifierType.Additive).Values.Sum();
            float multiAdd = _modiferGroup.GetModifier(EModifierType.MultiAdd).Values.Sum();
            float multiplicative = _modiferGroup.GetModifier(EModifierType.Multiplicative).Values.Aggregate(1f, (current, value) => current * (1 + value));

            foreach (var modifierGroup in _otherModGroups)
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
            _modiferGroup.Clear();
            _otherModGroups.Clear();

            Value = _initValue;
        }
        
        public static implicit operator float(BindableStat myObject)
        {
            return myObject.Value;
        }
        
        public static implicit operator int(BindableStat myObject)
        {
            return (int)myObject.Value;
        }
    }
}

