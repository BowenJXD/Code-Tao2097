using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao 
{
    public class Damager : ViewController
    {
        public Element DamageElement = new Element();
        public BindableStat DMG = new BindableStat();
        public BindableStat KnockBackFactor = new BindableStat();
        
        #region Condition
        
        public List<ETag> damagingTags = new List<ETag>();

        public float DMGCD;
        public bool IsInCD;

        public void StartCD()
        {
            if (DMGCD <= 0)
            {
                return;
            }
            
            IsInCD = true;
            ActionKit.Delay(DMGCD, () => { IsInCD = false; }).Start(this);
        }
        
        #endregion

        public bool ValidateDamage(Defencer defencer, Attacker attacker)
        {
            bool result = !IsInCD && Util.IsTagIncluded(ComponentUtil.GetTagFromParent(defencer), damagingTags);

            return result;
        }
        
        public Damage ProcessDamage(Damage damage)
        {
            damage.SetMedian(this);
            damage.SetBase(DMG.Value);
            damage.SetElement(DamageElement);
            damage.MultiplyKnockBack(KnockBackFactor);
            return damage;
        }
        
        public List<Func<Damage, Damage>> OnDealDamageFuncs = new List<Func<Damage, Damage>>();
        public Action<Damage> DealDamageAfter;
        
        public void DealDamage(Damage damage)
        {
            foreach (var func in OnDealDamageFuncs)
            {
                damage = func.Invoke(damage);
            }

            if (damage != null)
            {
                damage.Target.TakeDamage(damage);
                DealDamageAfter?.Invoke(damage);
            }
        }
    }

}

