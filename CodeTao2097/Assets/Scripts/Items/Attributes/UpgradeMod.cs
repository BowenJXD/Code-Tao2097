using System;
using UnityEngine;

namespace CodeTao
{
    [Serializable]
    public class UpgradeMod
    {
        /// <summary>
        /// Minimum level (inclusive) to unlock this modifier, 0 for default, negative to be overriden.
        /// </summary>
        public int minLevel = 0;
        /// <summary>
        /// Maximum level (inclusive) to unlock this modifier, 0 for default, negative to be overriden.
        /// </summary>
        public int maxLevel = 0;
        public EWAt attribute;
        public EModifierType modType;
        public float value;
        
        public bool CheckCondition(int level, bool triggered = false)
        {
            bool result = false;
            maxLevel = Mathf.Abs(maxLevel) < Mathf.Abs(minLevel) ? minLevel : maxLevel;
            if (triggered)
            {
                if (minLevel > 0 && maxLevel > 0)
                {
                    if (level >= Mathf.Abs(minLevel) && level <= Mathf.Abs(maxLevel))
                    {
                        result = true;
                    }
                }
            }
            else
            {
                if (level >= Mathf.Abs(minLevel) && level <= Mathf.Abs(maxLevel))
                {
                    result = true;
                }

                if (minLevel == 0 && maxLevel == 0)
                {
                    result = true;
                }
            }

            return result;
        }
        
        public static Comparison<UpgradeMod> Comparison()
        {
            return (a, b) =>
            {
                int absMinLevelComparison = Mathf.Abs(b.minLevel).CompareTo(Mathf.Abs(a.minLevel));

                if (absMinLevelComparison == 0)
                {
                    // If absolute minLevels are equal, positive comes before negative
                    return b.minLevel.CompareTo(a.minLevel);
                }
                else
                {
                    return absMinLevelComparison;
                }
            };
        }

        public BindableStat AddModifier(BindableStat stat, int newLevel)
        {
            stat.AddModifier($"Level{newLevel}", value, modType);

            return stat;
        }

        public string GetDescription()
        {
            string result = "";
            char sign = value > 0 ? '+' : '-';
            switch (modType)
            {
                case EModifierType.Basic:
                    result += $" base {attribute} {sign} {value}.";
                    break; 
                case EModifierType.Additive:
                    result += $" {attribute} {sign} {value}.";
                    break;
                case EModifierType.MultiAdd:
                    result += $" {attribute} {sign} {value * 100} %.";
                    break;
                case EModifierType.Multiplicative:
                    result += $" {attribute} * {(1 + value) * 100} %.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
    }
}