using System;
using CodeTao;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// Continuously deals damage to the target.
    /// </summary>
    public partial class DotBuff : Buff
    {
        [HideInInspector] private Defencer _target;
        [HideInInspector] public Damager damager;
        private BindableStat CritRate;
        private BindableStat CritDMG;
        public EAAt baseAttribute;
        public EModifierType modType;
        public bool canCrit;

        public override void Init()
        {
            base.Init();
            LVL.Value = 1;
            MaxLVL.Value = 5;

            if (!damager) { damager = this.GetComponentInDescendants<Damager>(); }

            if (damager)
            {
                CombatUnit combatUnit = Container.Unit as CombatUnit;
                if (combatUnit && baseAttribute != EAAt.Null)
                {
                    combatUnit.GetAAtMod(baseAttribute).RegisterWithInitValue(value =>
                    {
                        damager.DMG.AddModifier(value, modType, "BuffBaseAttribute", RepetitionBehavior.Overwrite, true);
                    }).UnRegisterWhenGameObjectDestroyed(this);
                }

                if (combatUnit && canCrit)
                {
                    CritRate = combatUnit.GetAAtMod(EAAt.CritRate);
                    CritDMG = combatUnit.GetAAtMod(EAAt.CritDamage);
                    damager.OnDealDamageFuncs.Add(damage =>
                    {
                        if (RandomUtil.RandCrit(CritRate.Value)){
                            damage.DamageSections[DamageSection.CRIT]["BuffCrit"] = CritDMG;
                        }
                        return damage;
                    });
                }
                
                LVL.RegisterWithInitValue(value =>
                {
                    damager.DMG.AddModifier(value, EModifierType.Multiplicative, "BuffLevel", RepetitionBehavior.Overwrite, true);
                }).UnRegisterWhenGameObjectDestroyed(this);
            }

            _target = buffOwner.GetComp<Defencer>();
            if (_target)
            {
                Trigger();
            }
        }
        
        public override void Trigger()
        {
            base.Trigger();
            DamageManager.Instance.ExecuteDamage(damager, _target);
        }
    }
}