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
        
        private float _hp;
        public float HP
        {
            get => _hp;
            set
            {
                SetHP(value);
            }
        }
        public BindableStat MaxHP = new BindableStat();

        private float SetHP(float value)
        {
            _hp = value;
            if (value <= 0)
            {
                
            }
            _hp = Mathf.Clamp(value, 0, MaxHP);
            return _hp;
        }

        private float AlterHP(float value)
        {
            return SetHP(HP + value);
        }
        
        #endregion
        
        #region Death
        
        public Action<Damage> OnDeath;
        
        public bool IsDead => HP <= 0;

        public void Die(Damage damage)
        {
            OnDeath.Invoke(damage);
        }
        
        #endregion
        
        #region Condition
        
        public float DMGCD;
        [HideInInspector] public bool IsInCD;
        
        public void StartCD()
        {
            IsInCD = true;
            ActionKit.Delay(DMGCD, () => { IsInCD = false; });
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
            damage.SetDamageSection(DamageSection.TargetDEF, "", DEF.Value);
            damage.SetDamageSection(DamageSection.ElementRES, "", ElementResistances[damage.DamageElement.Type], ERepetitionBehavior.Overwrite);
            damage.MultiplyKnockBack(KnockBackFactor);
            return damage;
        }
        
        public Func<Damage, Damage> OnTakeDamage;
        public Action<Damage> TakeDamageAfter;
        
        public void TakeDamage(Damage damage)
        {
            elementOwner?.AddElement(damage.DamageElement);
            
            damage = OnTakeDamage?.Invoke(damage);
            
            if (damage != null)
            {
                float damageValue = damage.CalculateDamage();
                AlterHP(-damageValue);
            }

            TakeDamageAfter?.Invoke(damage);
            
            if (IsDead)
            {
                Die(damage);
            }
        }
    }
}

