using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeTao;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    
    public partial class Attacker : ViewController
    {
        public BindableStat ATK = new BindableStat();
        public BindableStat CritRate = new BindableStat(); // 0% - 100%
        public BindableStat CritDamage = new BindableStat(); // %
        public Dictionary<ElementType, BindableStat> ElementBonuses = ElementType.GetValues(typeof(ElementType))
            .Cast<ElementType>()
            .ToDictionary(key => key, value => new BindableStat(0));

        public Damage ProcessDamage(Damage damage)
        {
            damage.SetSource(this);
            damage.SetDamageSection(DamageSection.SourceATK, "", ATK.Value);
            damage.SetDamageSection(DamageSection.CRIT, "", GetCritRate());
            damage.SetDamageSection(DamageSection.ElementBON, "", 1 + ElementBonuses[damage.DamageElement], ERepetitionBehavior.Overwrite);
            return damage;
        }
        
        public Action<Damage> DealDamageAfter;
        
        public float GetCritRate()
        {
            float result = 1.0f;
            if (Global.Instance.Random.Next(100) < CritRate.Value)
            {
                result = CritDamage.Value / 100;
            }
            return result;
        }

        private void OnDisable()
        {
            ATK.Reset();
            CritRate.Reset();
            CritDamage.Reset();
            foreach (var elementBonus in ElementBonuses)
            {
                elementBonus.Value.Reset();
            }
        }
    }
}