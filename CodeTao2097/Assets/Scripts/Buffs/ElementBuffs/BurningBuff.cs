using System;
using CodeTao;
using UnityEngine;

namespace Buffs.ElementBuffs
{
    public partial class BurningBuff : Buff
    {
        public BindableStat triggerInterval = new BindableStat(0.5f);
        public BindableStat duration = new BindableStat(5);
        public Defencer target;
        
        private void Init()
        {
            LVL.Value = 1;
            MaxLVL.Value = 5;
            BuffOwner buffOwner = (BuffOwner) Container;
            
            buffLoop = new LoopTask(buffOwner, triggerInterval, Burn, Remove);
            
            UnitController unit = ComponentUtil.GetComponentInAncestors<UnitController>(buffOwner);
            if (unit)
            {
                Defencer defencer = ComponentUtil.GetComponentInDescendants<Defencer>(unit);
                if (defencer)
                {
                    DamageManager.Instance.ExecuteDamage(Damager, defencer);
                }
            }

            RemoveAfter += iContent =>
            {
                buffLoop.Pause();
                buffLoop = null;
            };
        }
        
        public void Burn()
        {
            DamageManager.Instance.ExecuteDamage(Damager, target);
        }

        public void Remove()
        {
            IContent<Buff> iContent = this;
            iContent.RemoveFromContainer(Container);
        }
    }
}