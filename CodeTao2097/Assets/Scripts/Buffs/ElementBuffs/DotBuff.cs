using System;
using CodeTao;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public partial class DotBuff : Buff
    {
        [HideInInspector] private Defencer _target;
        [HideInInspector] private Damager _damager;

        public override void Init()
        {
            base.Init();
            LVL.Value = 1;
            MaxLVL.Value = 5;
            
            _damager = ComponentUtil.GetComponentInDescendants<Damager>(this);
            Attacker attacker = ComponentUtil.GetComponentFromUnit<Attacker>(Container);
            _damager.DMG.AddModifier(attacker.ATK, EModifierType.Multiplicative, "Attacker's ATK", ERepetitionBehavior.Overwrite);

            LVL.RegisterWithInitValue(value =>
            {
                _damager.DMG.AddModifier(LVL, EModifierType.Multiplicative, "BuffLevel", ERepetitionBehavior.Overwrite);
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            
            _target = ComponentUtil.GetComponentFromUnit<Defencer>(buffOwner);
            if (_target)
            {
                Trigger();
            }
        }
        
        public override void Trigger()
        {
            base.Trigger();
            DamageManager.Instance.ExecuteDamage(_damager, _target);
        }

    }
}