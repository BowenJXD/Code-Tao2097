using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CodeTao;
using JetBrains.Annotations;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 伤害乘区
    /// </summary>
    public enum DamageSection
    {
        SourceATK,
        TargetDEF,
        CRIT,
        DamageIncrement,
        /// <summary>
        /// Defaults to be 0
        /// </summary>
        DamageDecrement,
        ReactionMultiplier
    }

    /// <summary>
    /// 伤害，包括Attacker、Damager、Defencer、伤害元素、基础伤害、伤害乘区。。。最终伤害值由基础伤害乘以所有乘区的乘积得到。
    /// </summary>
    public class Damage
    {
        public Attacker Source { get; private set; }
        public Damager Median { get; private set; }
        public Defencer Target { get; private set; }
        public ElementType DamageElement { get; private set; }
        public float Base { get; private set; }
        public float Final { get; private set; }
        public bool Dealt { get; private set; }
        public List<DamageTag> damageTags = new List<DamageTag>();
        
        public Dictionary<DamageSection, Dictionary<string, float>> DamageSections = DamageSection.GetValues(typeof(DamageSection))
            .Cast<DamageSection>()
            .ToDictionary(key => key, value => new Dictionary<string, float>());

        public float Knockback = 1;

        public Damage SetSource(Attacker source)
        {
            Source = source;
            return this;
        }
        
        public Damage SetMedian(Damager median)
        {
            Median = median;
            return this;
        }
        
        public Damage SetTarget(Defencer target)
        {
            Target = target;
            return this;
        }
        
        public Damage SetElement(ElementType element)
        {
            DamageElement = element;
            return this;
        }
        
        public Damage SetBase(float value)
        {
            Base = value;
            return this;
        }
        
        public Damage SetDealt(bool value)
        {
            Dealt = value;
            return this;
        }

        public bool SetDamageSection(DamageSection section, string name, float value, RepetitionBehavior repetitionBehavior = RepetitionBehavior.Return)
        {
            if (DamageSections.ContainsKey(section))
            {
                if (DamageSections[section].ContainsKey(name))
                {
                    switch (repetitionBehavior)
                    {
                        case RepetitionBehavior.Return:
                            break;
                        case RepetitionBehavior.Overwrite:
                            DamageSections[section][name] = value;
                            break;
                        case RepetitionBehavior.AddStack:
                            DamageSections[section][name] += value;
                            break;
                    }

                    return false;
                }
                
                return DamageSections[section].TryAdd(name, value);
            }

            return false;
        }

        public float GetDamageSection(DamageSection section)
        {
            if (DamageSections.ContainsKey(section))
            {
                return DamageSections[section].Values.Sum();
            }

            return 1;
        }
            
        public bool RemoveDamageSection(DamageSection section, string name)
        {
            if (DamageSections.ContainsKey(section))
            {
                return DamageSections[section].Remove(name);
            }

            return false;
        }
        
        public Damage MultiplyKnockBack(float value)
        {
            Knockback *= value;
            return this;
        }
        
        public void AddDamageTag(DamageTag damageTag)
        {
            if (!damageTags.Contains(damageTag))
                damageTags.Add(damageTag);
        }
        
        public void RemoveDamageTag(DamageTag damageTag)
        {
            if (damageTags.Contains(damageTag))
                damageTags.Remove(damageTag);
        }
        
        public bool HasDamageTag(DamageTag damageTag)
        {
            return damageTags.Contains(damageTag);
        }
        
        public void CalculateDamageValue()
        {
            float result = Base;
            foreach (var damageSection in DamageSections)
            {
                if (damageSection.Value.Count > 0)
                {
                    result *= damageSection.Value.Values.Sum();
                }
            }
            Final = result;
        }

        public override string ToString()
        {
            return Final.ToString(CultureInfo.CurrentCulture);
        }

        public static string GetCSVHeader()
        {
            return
                "Median,Target,Source,Element,Damage Value,Base Value,SourceATK,TargetDEF,CRIT,DamageIncrement,DamageDecrement,ReactionMultiplier,KnockBack,DamageTags";
        }
        
        public string ToCSV()
        {
            string log = "";
            log += $"{Median.name},{Target.name},{(Source?Source.name : "")},{DamageElement},{this},{Base},";
            foreach (var damageSection in DamageSections)
            {
                if (damageSection.Value.Count > 0)
                {
                    log += $"{damageSection.Value.Values.Sum()},";
                }
                else
                {
                    log += ",";
                }
            }
            log += $"{Knockback},";
            foreach (var damageTag in damageTags)
            {
                log += $"{damageTag}\n";
            }
            return log;
        }
    }

    
}