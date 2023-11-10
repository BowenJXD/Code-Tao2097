using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public enum ModifierType
    {
        Basic,
        Multiplicative,
        Additive
    }

    public enum RepetitionBehavior
    {
        Return,
        Overwrite,
        Stack
    }
    
    public class BindableStat : BindableProperty<float>
    {
        protected Dictionary<string, float> mBasicModifiers = new Dictionary<string, float>();
        protected Dictionary<string, float> mAdditiveModifiers = new Dictionary<string, float>();
        protected Dictionary<string, float> mMultiplicativeModifiers = new Dictionary<string, float>();
        
        
        
        public bool AddModifier(string name, float value, ModifierType modifierType, RepetitionBehavior repetitionBehavior = RepetitionBehavior.Return)
        {
            Dictionary<string, float> modifiers = null;
            switch (modifierType)
            {
                case ModifierType.Basic:
                    modifiers = mBasicModifiers;
                    break;
                case ModifierType.Additive:
                    modifiers = mAdditiveModifiers;
                    break;
                case ModifierType.Multiplicative:
                    modifiers = mMultiplicativeModifiers;
                    break;
            }
            
            if (modifiers == null)
            {
                return false;
            }
            
            if (modifiers.ContainsKey(name))
            {
                switch (repetitionBehavior)
                {
                    case RepetitionBehavior.Return:
                        break;
                    case RepetitionBehavior.Overwrite:
                        modifiers[name] = value;
                        break;
                    case RepetitionBehavior.Stack:
                        modifiers[name] += value;
                        break;
                }

                return false;
            }

            return modifiers.TryAdd(name, value);
        }
        
        public bool RemoveModifier(string name, ModifierType modifierType)
        {
            switch (modifierType)
            {
                case ModifierType.Basic:
                    return mBasicModifiers.Remove(name);
                case ModifierType.Additive:
                    return mAdditiveModifiers.Remove(name);
                case ModifierType.Multiplicative:
                    return mMultiplicativeModifiers.Remove(name);
            }

            return false;
        }

        protected override float GetValue()
        {
            float basic = mBasicModifiers.Values.Sum();
            float additive = mAdditiveModifiers.Values.Sum();
            float multiplicative = mMultiplicativeModifiers.Values.Sum();
            return (mValue + basic) * multiplicative + additive;
        }
    }
}

