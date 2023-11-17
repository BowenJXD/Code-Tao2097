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
        [HideInInspector] public bool IsInCD;

        public void StartCD()
        {
            IsInCD = true;
            ActionKit.Delay(DMGCD, () => { IsInCD = false; });
        }
        
        #endregion

        public bool ValidateDamage(Defencer defencer, Attacker attacker)
        {
            bool result = !IsInCD && Util.IsTagIncluded(Util.GetTagFromParent(defencer), damagingTags);

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
        
        public Func<Damage, Damage> OnDealDamage;
        public Action<Damage> DealDamageAfter;
        
        public void DealDamage(Damage damage)
        {
            OnDealDamage.Invoke(damage);
            damage.Target.TakeDamage(damage);
            DealDamageAfter.Invoke(damage);
        }
    }

}

