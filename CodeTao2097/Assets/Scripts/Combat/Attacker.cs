using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeTao;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public enum AttackerUsage
    {
        ATK,
        Crit,
        ElementBonus,
    }
    
    /// <summary>
    /// 增幅伤害的组件，通常挂载在有defencer的单位上。包括攻击力、暴击率、暴击伤害、五行伤害加成，不为造成伤害的必要条件。
    /// </summary>
    public partial class Attacker : UnitComponent, IAAtReceiver
    {
        public BindableStat ATK = new BindableStat();
        public BindableStat CritRate = new BindableStat(); // 0% - 100%
        public BindableStat CritDamage = new BindableStat(); // %
        public Dictionary<ElementType, BindableStat> ElementBonuses = Enum.GetValues(typeof(ElementType))
            .Cast<ElementType>()
            .ToDictionary(key => key, value => new BindableStat(0));
        
        public List<Func<Damage, Damage>> onDealDamageFuncs = new List<Func<Damage, Damage>>();
        
        public Damage ProcessDamage(Damage damage, AttackerUsage[] attackerUsages = null)
        {
            if (attackerUsages == null)
            {
                attackerUsages = new AttackerUsage[] { AttackerUsage.ATK, AttackerUsage.Crit, AttackerUsage.ElementBonus };
            }
            
            damage.SetSource(this);
            
            if (attackerUsages.Contains(AttackerUsage.ATK)) damage.SetDamageSection(DamageSection.SourceStat, "", ATK.Value);
            
            if (attackerUsages.Contains(AttackerUsage.Crit)){
                float critRate = GetCritRate();
                if (critRate > 1)
                {
                    damage.SetDamageSection(DamageSection.CRIT, "", critRate);
                    damage.AddDamageTag(DamageTag.Critical);
                }
            }

            if (attackerUsages.Contains(AttackerUsage.ElementBonus)){
                damage.SetDamageSection(DamageSection.DamageIncrement, "",
                    1 + ElementBonuses[damage.DamageElement] + ElementBonuses[ElementType.All],
                    RepetitionBehavior.Overwrite);
            }
            
            return damage;
        }

        public Damage ProcessDamageExt(Damage damage)
        {
            foreach (var onDealDamageFunc in onDealDamageFuncs)
            {
                damage = onDealDamageFunc(damage);
            }
            
            return damage;
        }
        
        public Action<Damage> DealDamageAfter;
        
        public float GetCritRate()
        {
            float result = 1.0f;
            if (RandomUtil.Rand100(CritRate.Value))
            {
                result = CritDamage.Value / 100;
            }
            return result;
        }

        private void OnEnable()
        {
            ATK.Init();
            CritRate.Init();
            CritDamage.Init();
            foreach (var elementBonus in ElementBonuses)
            {
                elementBonus.Value.Init();
            }
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
            ElementBonuses[ElementType.Metal].InheritStat(source.GetAAt(EAAt.MetalElementBON), true);
            ElementBonuses[ElementType.Fire].InheritStat(source.GetAAt(EAAt.FireElementBON), true);
            ElementBonuses[ElementType.Water].InheritStat(source.GetAAt(EAAt.WaterElementBON), true);
            ElementBonuses[ElementType.Wood].InheritStat(source.GetAAt(EAAt.WoodElementBON), true);
            ElementBonuses[ElementType.Earth].InheritStat(source.GetAAt(EAAt.EarthElementBON), true);
        }
    }
}