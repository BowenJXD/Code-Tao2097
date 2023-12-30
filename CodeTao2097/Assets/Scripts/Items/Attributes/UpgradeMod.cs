using System;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public abstract class UpgradeMod
    {
        /// <summary>
        /// Each digit in levels represents a level of upgrade, "" to include all levels > 1, 0 to include all levels, -x to include all levels > x.
        /// </summary>
        [Tooltip("Each digit in levels represents a level of upgrade, \"\" to include all levels > 1, 0 to include all levels, -x to include all levels > x.")]
        public string levels = "";
        /// <summary>
        /// If exclusive, stop checking other mods when this mod is triggered.
        /// </summary>
        public bool exclusive = true;
        public float value;
        public EModifierType modType;

        public abstract string GetAttribute();
        
        public bool CheckCondition(int level)
        {
            bool result = false;

            int levelDigit = -1;
            if (levels != "")
            {
                int.TryParse(levels, out levelDigit);
            }

            if (levelDigit <= 0)
            {
                result = level > -levelDigit;
            }
            else
            {
                foreach (char c in levels)
                {
                    int digit = c - '0';
                    if (digit == level)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        public string GetDescription()
        {
            string result = "";
            char sign = value > 0 ? '+' : '-';
            switch (modType)
            {
                case EModifierType.Basic:
                    result += $" base {GetAttribute()} {sign} {value}.";
                    break; 
                case EModifierType.Additive:
                    result += $" {GetAttribute()} {sign} {value}.";
                    break;
                case EModifierType.MultiAdd:
                    result += $" {GetAttribute()} {sign} {value * 100} %.";
                    break;
                case EModifierType.Multiplicative:
                    result += $" {GetAttribute()} * {(1 + value) * 100} %.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
    }
}