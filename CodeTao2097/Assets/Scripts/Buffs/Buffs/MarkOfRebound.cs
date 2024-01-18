using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 痕印（弹射物增伤）Mark of Rebound（水）
    /// 附属者受到弹射物的伤害后，令其弹射物的伤害增加。
    /// </summary>
    public class MarkOfRebound : Buff
    {
        public float damageIncrement = 0.1f;
        public EModifierType modifierType = EModifierType.MultiAdd;
        public RepetitionBehavior repetitionBehavior = RepetitionBehavior.AddStack;
        
        protected Defencer defencer;
        List<Damager> damagers = new List<Damager>();
        
        public override void OnAdd()
        {
            base.OnAdd();
            defencer = Container.GetComp<Defencer>();
            
            if (defencer)
            {
                defencer.TakeDamageAfter += OnTakeDamage;
            }
        }
        
        public void OnTakeDamage(Damage damage)
        {
            Damager damager = damage.Median;
            Projectile projectile = damager.Unit as Projectile;
            if (projectile)
            {
                if (damager.DMG.AddModifier(damageIncrement * LVL, modifierType, name, repetitionBehavior));
                {
                    damagers.Add(damager);
                    projectile.onDeinit += () =>
                    {
                        damager.DMG.RemoveModifier(modifierType, name);
                        damagers.Remove(damager);
                    };
                }
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            if (defencer)
            {
                defencer.TakeDamageAfter -= OnTakeDamage;
            }
            foreach (var damager in damagers)
            {
                damager.DMG.RemoveModifier(modifierType, name);
            }
            damagers.Clear();
        }
    }
}