using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeTao;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 增幅伤害的组件，通常挂载在有defencer的单位上。包括攻击力、暴击率、暴击伤害、五行伤害加成，不为造成伤害的必要条件。
    /// </summary>
    public partial class Attacker : UnitComponent, IAAtReceiver
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
            float critRate = GetCritRate();
            if (critRate > 1)
            {
                damage.SetDamageSection(DamageSection.CRIT, "", critRate);
                damage.AddDamageTag(DamageTag.Critical);
            }
            damage.SetDamageSection(DamageSection.DamageIncrement, "", 1 + ElementBonuses[damage.DamageElement], RepetitionBehavior.Overwrite);
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

        public void Receive(IAAtSource source)
        {
            ATK.InheritStat(source.GetAAt(EAAt.ATK));
            CritRate.InheritStat(source.GetAAt(EAAt.CritRate));
            CritDamage.InheritStat(source.GetAAt(EAAt.CritDamage));
            ElementBonuses[ElementType.Metal].InheritStat(source.GetAAt(EAAt.MetalElementBON));
            ElementBonuses[ElementType.Fire].InheritStat(source.GetAAt(EAAt.FireElementBON));
            ElementBonuses[ElementType.Water].InheritStat(source.GetAAt(EAAt.WaterElementBON));
            ElementBonuses[ElementType.Wood].InheritStat(source.GetAAt(EAAt.WoodElementBON));
            ElementBonuses[ElementType.Earth].InheritStat(source.GetAAt(EAAt.EarthElementBON));
        }
    }
}