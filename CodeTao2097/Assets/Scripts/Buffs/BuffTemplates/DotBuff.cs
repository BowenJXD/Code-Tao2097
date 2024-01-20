using System;
using CodeTao;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 持续伤害buff
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
                damager.AddDamageTag(DamageTag.Dot);
                AttributeController attributeController = buffOwner.GetComp<AttributeController>();
                if (attributeController && baseAttribute != EAAt.Null)
                {
                    attributeController.GetAAt(baseAttribute).RegisterWithInitValue(value =>
                    {
                        damager.DMG.AddModifier(value, modType, "BuffBaseAttribute", RepetitionBehavior.Overwrite, true);
                    }).UnRegisterWhenGameObjectDestroyed(this);
                }

                if (attributeController && canCrit)
                {
                    CritRate = attributeController.GetAAt(EAAt.CritRate);
                    CritDMG = attributeController.GetAAt(EAAt.CritDamage);
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