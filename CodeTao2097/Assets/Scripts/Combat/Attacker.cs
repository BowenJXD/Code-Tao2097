using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeTao;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    
    public class Attacker : ViewController
    {
        public BindableStat ATK = new BindableStat();
        public BindableStat CritRate = new BindableStat(); // 0% - 100%
        public BindableStat CritDamage = new BindableStat(); // %
        public Dictionary<ElementType, float> ElementBonuses = ElementType.GetValues(typeof(ElementType))
            .Cast<ElementType>()
            .ToDictionary(key => key, value => 0.0f);

        public Damage ProcessDamage(Damage damage)
        {
            damage.SetSource(this);
            damage.SetDamageSection(DamageSection.SourceATK, "", ATK.Value);
            damage.SetDamageSection(DamageSection.CRIT, "", GetCritRate());
            damage.SetDamageSection(DamageSection.ElementBON, "", 1 + ElementBonuses[damage.DamageElement.Type], ERepetitionBehavior.Overwrite);
            return damage;
        }
        
        public float GetCritRate()
        {
            float result = 1.0f;
            if (Global.Instance.Random.Next(100) < CritRate.Value)
            {
                result = CritDamage.Value / 100;
            }
            return result;
        }
    }

}