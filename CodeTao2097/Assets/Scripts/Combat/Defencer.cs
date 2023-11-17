using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public partial class Defencer : ViewController
    {
        #region HP
        
        public BindableProperty<float> HP;
        public BindableStat MaxHP = new BindableStat();

        private float SetHP(float value)
        {
            if (value <= 0)
            {
                
            }
            HP.Value = Mathf.Clamp(value, 0, MaxHP);
            return HP.Value;
        }

        private float AlterHP(float value)
        {
            return SetHP(HP.Value + value);
        }
        
        #endregion
        
        #region Death
        
        public Action<Damage> OnDeath;
        
        public bool IsDead => HP.Value <= 0;

        public void Die(Damage damage)
        {
            OnDeath?.Invoke(damage);
            if (IsDead)
            {
                OnDeath = null;
            }
        }
        
        #endregion
        
        #region Condition
        
        public float DMGCD;
        [HideInInspector] public bool IsInCD;
        
        public void StartCD()
        {
            if (DMGCD <= 0)
            {
                return;
            }
            
            IsInCD = true;
            ActionKit.Delay(DMGCD, () => { IsInCD = false; }).Start(this);
        }
        
        public List<ETag> defencingTags = new List<ETag>();
        
        #endregion

        #region Taking
        
        public BindableStat DEF = new BindableStat();
        
        public Dictionary<ElementType, float> ElementResistances = ElementType.GetValues(typeof(ElementType))
            .Cast<ElementType>()
            .ToDictionary(key => key, value => 0.0f);
        
        public BindableStat KnockBackFactor = new BindableStat();

        #endregion
        
        public ElementOwner elementOwner;
        
        public bool ValidateDamage(Damager damager, Attacker attacker)
        {
            bool result = !IsInCD && !Util.IsTagIncluded(Util.GetTagFromParent(damager), defencingTags);

            return result;
        }
        
        public Damage ProcessDamage(Damage damage)
        {
            damage.SetTarget(this);
            damage.SetDamageSection(DamageSection.TargetDEF, "", 1 / DEF.Value);
            damage.SetDamageSection(DamageSection.ElementRES, "", 1 - ElementResistances[damage.DamageElement.Type], ERepetitionBehavior.Overwrite);
            damage.MultiplyKnockBack(KnockBackFactor);
            return damage;
        }
        
        public Func<Damage, Damage> OnTakeDamage;
        public Action<Damage> TakeDamageAfter;
        
        public void TakeDamage(Damage damage)
        {
            elementOwner?.AddElement(damage.DamageElement);

            if (OnTakeDamage != null)
            {
                damage = OnTakeDamage.Invoke(damage);
            }

            if (damage != null)
            {
                float damageValue = damage.CalculateDamage();
                AlterHP(-damageValue);
                TakeDamageAfter?.Invoke(damage);
            }
            
            if (IsDead)
            {
                Die(damage);
            }
        }
    }
}
