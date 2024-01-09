using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeTao
{
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
}