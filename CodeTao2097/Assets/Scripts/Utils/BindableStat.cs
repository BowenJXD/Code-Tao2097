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

        public bool AddModifier(string name, float value, EModifierType modifierType, 
            ERepetitionBehavior repetitionBehavior = ERepetitionBehavior.Return)
        {
            bool result = false;
            Dictionary<string, float> modifiers = _modifiers[modifierType];
            
            if (modifiers == null)
            {
                return result;
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

            result = modifiers.TryAdd(name, value);
            
            return result;
        }
        
        public bool RemoveModifier(string name, EModifierType modifierType)
        {
            bool result = false;
            result = _modifiers[modifierType].Remove(name);
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

        public BindableStat()
        {
            _initValue = mValue;
        }
        
        public BindableStat(float value) : base(value)
        {
            _initValue = mValue;
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

        public bool AddModifier(string name, float value, EModifierType modifierType,
            ERepetitionBehavior repetitionBehavior = ERepetitionBehavior.Return)
        {
            bool result = _modiferGroup.AddModifier(name, value, modifierType, repetitionBehavior);
            if (result)
            {
                mOnValueChanged?.Invoke(Value);
            }

            return result;
        }

        public bool RemoveModifier(string name, EModifierType modifierType)
        {
            bool result = false;
            result = _modiferGroup.RemoveModifier(name, modifierType);

            if (result)
            {
                mOnValueChanged?.Invoke(Value);
            }

            return result;
        }

        protected override float GetValue()
        {
            float basic = _modiferGroup.GetModifier(EModifierType.Basic).Values.Sum();
            float additive = _modiferGroup.GetModifier(EModifierType.Additive).Values.Sum();
            float multiAdd = _modiferGroup.GetModifier(EModifierType.MultiAdd).Values.Sum();
            float multiplicative = _modiferGroup.GetModifier(EModifierType.Multiplicative).Values.Aggregate(1f, (current, value) => current * (1 + value));
            float resultValue = (mValue + basic) * multiplicative * (1 + multiAdd) + additive;
            resultValue = Math.Clamp(resultValue, _minValue, _maxValue);
            return resultValue;
        }
        
        public void Reset()
        {
            _modiferGroup.Clear();

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

