using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 受到伤害的组件。包括血量，防御，抗性，伤害cd，以及受到伤害的逻辑。为造成伤害的必要条件。
    /// </summary>
    public partial class Defencer : UnitComponent
    {
        #region HP
        
        public BindableStat Lives = new BindableStat(1);
        public BindableStat HP = new BindableStat();
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
            Lives.Value--;
            if (Lives.Value <= 0)
            {
                OnDeath?.Invoke(damage);
                if (IsDead)
                {
                    OnDeath = null;
                }
            }
            else
            {
                SetHP(MaxHP);
            }
        }
        
        #endregion
        
        #region Condition
        
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
        
        public List<EntityType> defencingTags = new List<EntityType>();
        
        #endregion

        #region Taking
        
        public BindableStat DEF = new BindableStat().SetMinValue(0);
        
        public Dictionary<ElementType, BindableStat> ElementResistances = ElementType.GetValues(typeof(ElementType))
            .Cast<ElementType>()
            .ToDictionary(key => key, value => new BindableStat(0));
        
        public BindableStat KnockBackFactor = new BindableStat(1);

        protected Rigidbody2D rb;

        #endregion
        
        private void OnEnable()
        {
            MaxHP.RegisterWithInitValue(value =>
            {
                HP.SetMaxValue(value);
            }).UnRegisterWhenGameObjectDestroyed(this);
            HP.Value = MaxHP;
            
            rb = this.GetComponentFromUnit<Rigidbody2D>();
        }
        
        public bool ValidateDamage(Damager damager, Attacker attacker)
        {
            bool result = !IsInCD && !Util.IsTagIncluded(Unit.tag, defencingTags);

            return result;
        }
        
        public Damage ProcessDamage(Damage damage)
        {
            damage.SetTarget(this);
            var def = DEF.Value;
            damage.SetDamageSection(DamageSection.TargetDEF, "", 1 - def / (Global.Instance.DefenceFactor + def));
            damage.SetDamageSection(DamageSection.DamageDecrement, "", 1 - ElementResistances[damage.DamageElement], RepetitionBehavior.Overwrite);
            damage.MultiplyKnockBack(KnockBackFactor);
            return damage;
        }
        
        public List<Func<Damage, Damage>> OnTakeDamageFuncs = new List<Func<Damage, Damage>>();
        public Action<Damage> TakeDamageAfter;
        
        
        public void TakeDamage(Damage damage)
        {
            foreach (var func in OnTakeDamageFuncs)
            {
                damage = func.Invoke(damage);
            }

            if (damage != null)
            {
                float damageValue = damage.GetDamageValue();
                if (damageValue > 0)
                {
                    AlterHP(-damageValue);
                    damage.SetDealt(true);
                }
                TakeDamageAfter?.Invoke(damage);
            }
            
            if (IsDead)
            {
                Die(damage);
            }
            else if (rb && damage is { Knockback: > 0 })
            {
                Vector3 knockBackSource = damage.Source ? damage.Source.transform.position : damage.Median.transform.position;
                Vector3 sourceTargetDistance = transform.position - knockBackSource;
                if (sourceTargetDistance.magnitude > 0.1f)
                {
                    Vector2 knockBackDirection = sourceTargetDistance.normalized;
                    rb.AddForce(damage.Knockback * knockBackDirection, ForceMode2D.Impulse);
                }
            }
        }

        private void OnDisable()
        {
            MaxHP.Reset();
            DEF.Reset();
            KnockBackFactor.Reset();
            foreach (var elementResistance in ElementResistances)
            {
                elementResistance.Value.Reset();
            }
            
            OnTakeDamageFuncs.Clear();
            
            IsInCD = false;
        }
    }
}

