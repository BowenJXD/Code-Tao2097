using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeTao;
using JetBrains.Annotations;
using UnityEngine;

namespace CodeTao
{
    public enum DamageSection
    {
        SourceATK,
        TargetDEF,
        CRIT,
        ElementBON,
        ElementRES,
        ReactionMultiplier
    }

    public class DamageSectionList
    {
        public DamageSection Section;
        public Dictionary<string, float> Modifiers = new Dictionary<string, float>();
    }
    
    public class Damage
    {
        protected Attacker Source;
        protected Damager Median;
        protected Defencer Target;
        public Element DamageElement { get; protected set; } = new Element();
        protected float Base = 0; 
        protected Dictionary<DamageSection, Dictionary<string, float>> DamageSections = DamageSection.GetValues(typeof(DamageSection))
            .Cast<DamageSection>()
            .ToDictionary(key => key, value => new Dictionary<string, float>());

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
        
        public Damage SetElement(Element element)
        {
            DamageElement = element;
            return this;
        }
        
        public Damage SetBase(float value)
        {
            Base = value;
            return this;
        }

        public bool SetDamageSection(DamageSection section, string modifiername, float value, RepetitionBehavior repetitionBehavior = RepetitionBehavior.Return)
        {
            if (DamageSections.ContainsKey(section))
            {
                if (DamageSections[section].ContainsKey(modifiername))
                {
                    switch (repetitionBehavior)
                    {
                        case RepetitionBehavior.Return:
                            break;
                        case RepetitionBehavior.Overwrite:
                            DamageSections[section][modifiername] = value;
                            break;
                        case RepetitionBehavior.Stack:
                            DamageSections[section][modifiername] += value;
                            break;
                    }

                    return false;
                }
                
                return DamageSections[section].TryAdd(modifiername, value);
            }

            return false;
        }

        public float CalculateDamage()
        {
            float result = Base;
            foreach (var damageSection in DamageSections)
            {
                if (damageSection.Value.Count > 0)
                {
                    result *= damageSection.Value.Values.Sum();
                }
            }

            return result;
        }
    }

    
}