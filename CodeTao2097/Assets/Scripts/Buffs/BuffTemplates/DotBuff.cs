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
        public EAAt baseAttribute;
        public EModifierType modType;

        public override void Init()
        {
            base.Init();
            LVL.Value = 1;
            MaxLVL.Value = 5;

            if (!damager)
            {
                damager = this.GetComponentInDescendants<Damager>();
                if (baseAttribute != EAAt.Null)
                {
                    CombatUnit combatUnit = Container.GetComponentInAncestors<CombatUnit>();
                    combatUnit.GetAAtMod(baseAttribute).RegisterWithInitValue(value =>
                    {
                        damager.DMG.AddModifier(value, modType, "BuffBaseAttribute", RepetitionBehavior.Overwrite);
                    }).UnRegisterWhenGameObjectDestroyed(this);
                }
                
                LVL.RegisterWithInitValue(value =>
                {
                    damager.DMG.AddModifier(value, EModifierType.Multiplicative, "BuffLevel", RepetitionBehavior.Overwrite);
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