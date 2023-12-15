using System;
using CodeTao;
using UnityEngine;

namespace Buffs.ElementBuffs
{
    public partial class BurningBuff : Buff
    {
        public BindableStat triggerInterval = new BindableStat(0.5f);
        public BindableStat duration = new BindableStat(5);
        [HideInInspector] private Defencer _target;
        [HideInInspector] private Damager _damager;

        public override void OnAdd()
        {
            base.OnAdd();
            Init();
        }

        private void Init()
        {
            LVL.Value = 1;
            MaxLVL.Value = 5;
            _damager = ComponentUtil.GetComponentInDescendants<Damager>(this);
            Attacker attacker = ComponentUtil.GetComponentFromUnit<Attacker>(Container);
            _damager.DMG.AddModifier("Attacker", attacker.ATK, EModifierType.Multiplicative);
            
            BuffOwner buffOwner = (BuffOwner) Container;
            
            _target = ComponentUtil.GetComponentFromUnit<Defencer>(buffOwner);
            if (_target)
            {
                Burn();
            }
            buffLoop = new LoopTask(buffOwner, triggerInterval, Burn, Remove);
            buffLoop.Start();
        }
        
        public void Burn()
        {
            DamageManager.Instance.ExecuteDamage(_damager, _target);
        }

        public void Remove()
        {
            buffLoop = null;
            Content<Buff> iContent = this;
            iContent.RemoveFromContainer(Container);
        }
    }
}