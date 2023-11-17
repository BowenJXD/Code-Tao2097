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
    
    [Serializable]
    public class BindableStat : BindableProperty<float>
    {
        private float _initValue = 0;
        private Dictionary<string, float> _basicModifiers = new Dictionary<string, float>();
        private Dictionary<string, float> _additiveModifiers = new Dictionary<string, float>();
        private Dictionary<string, float> _multiAddModifiers = new Dictionary<string, float>();
        private Dictionary<string, float> _multiplicativeModifiers = new Dictionary<string, float>();
        private float _minValue = 0;
        private float _maxValue = float.MaxValue;

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
        
        public bool AddModifier(string name, float value, EModifierType modifierType, ERepetitionBehavior repetitionBehavior = ERepetitionBehavior.Return)
        {
            bool result = false;
            Dictionary<string, float> modifiers = null;
            switch (modifierType)
            {
                case EModifierType.Basic:
                    modifiers = _basicModifiers;
                    break;
                case EModifierType.Additive:
                    modifiers = _additiveModifiers;
                    break;
                case EModifierType.MultiAdd:
                    modifiers = _multiAddModifiers;
                    break;
                case EModifierType.Multiplicative:
                    modifiers = _multiplicativeModifiers;
                    break;
            }
            
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
                        modifiers.Add(name, value);
                        result = true;
                        break;
                }
            }

            result = modifiers.TryAdd(name, value);
            
            if (result)
            {
                mOnValueChanged?.Invoke(Value);
            }
            
            return result;
        }
        
        public bool RemoveModifier(string name, EModifierType modifierType)
        {
            bool result = false;
            switch (modifierType)
            {
                case EModifierType.Basic:
                    result = _basicModifiers.Remove(name);
                    break;
                case EModifierType.Additive:
                    result = _additiveModifiers.Remove(name);
                    break;
                case EModifierType.MultiAdd:
                    result = _multiAddModifiers.Remove(name);
                    break;
                case EModifierType.Multiplicative:
                    result = _multiplicativeModifiers.Remove(name);
                    break;
            }

            if (result)
            {
                mOnValueChanged?.Invoke(Value);
            }
            return result;
        }

        protected override float GetValue()
        {
            float basic = _basicModifiers.Values.Sum();
            float additive = _additiveModifiers.Values.Sum();
            float multiAdd = _multiAddModifiers.Values.Sum();
            float multiplicative = _multiplicativeModifiers.Values.Aggregate(1f, (current, value) => current * (1 + value));
            float resultValue = (mValue + basic) * multiplicative * (1 + multiAdd) + additive;
            resultValue = Math.Clamp(resultValue, _minValue, _maxValue);
            return resultValue;
        }
        
        public void Reset()
        {
            _basicModifiers.Clear();
            _additiveModifiers.Clear();
            _multiplicativeModifiers.Clear();
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

