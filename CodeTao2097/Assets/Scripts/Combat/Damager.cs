using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao 
{
    /// <summary>
    /// 造成伤害的组件。通常挂载在武器或武器衍生物上。包括伤害数值、伤害类型、伤害间隔等。为造成伤害的必要条件。
    /// </summary>
    public class Damager : UnitComponent
    {
        public ElementType damageElementType = ElementType.None;
        public BindableStat DMG = new BindableStat(-1);
        public BindableStat knockBackFactor = new BindableStat(-1);
        
        public BindableStat effectHitRate = new BindableStat(-1).SetMaxValue(100f);
        public BindableStat effectDuration = new BindableStat(-1).SetMinValue(0.1f);
        public Buff buffToApply;
        private ContentPool<Buff> _buffPool;
        
        #region Condition
        
        public List<EntityType> damagingTags = new List<EntityType> { EntityType.Enemy };

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

        private void OnEnable()
        {
            if (!buffToApply) buffToApply = this.GetComponentInDescendants<Buff>();
            if (buffToApply && _buffPool == null) _buffPool = new ContentPool<Buff>(buffToApply);
        }

        public bool ValidateDamage(Defencer defencer, Attacker attacker)
        {
            UnitController unit = defencer.Unit;
            bool result = !IsInCD && Util.IsTagIncluded(unit.tag, damagingTags);

            return result;
        }
        
        public Damage ProcessDamage(Damage damage)
        {
            damage.SetMedian(this);
            damage.SetBase(DMG.Value);
            damage.SetElement(damageElementType);
            damage.MultiplyKnockBack(knockBackFactor);
            foreach (var func in OnDealDamageFuncs)
            {
                damage = func.Invoke(damage);
            }
            return damage;
        }
        
        public List<Func<Damage, Damage>> OnDealDamageFuncs = new List<Func<Damage, Damage>>();
        public Action<Damage> DealDamageAfter;
        
        public void DealDamage(Damage damage)
        {
            if (damage != null)
            {
                damage.Target.TakeDamage(damage);
                TryApplyBuff(damage);
                DealDamageAfter?.Invoke(damage);
            }
        }
        
        public virtual void TryApplyBuff(Damage damage)
        {
            if (damage.Target.IsDead) return;
            BuffOwner target = damage.Target.GetComp<BuffOwner>();
            if (target && buffToApply && CheckBuffHit(damage))
            {
                ApplyBuff(target);
            }
        }
        
        public virtual Buff ApplyBuff(BuffOwner target)
        {
            Buff buff = _buffPool.Get().Parent(this);
            buff.duration.AddModifierGroups(effectDuration.ModGroups);
            
            if (!buff.AddToContainer(target))
            {
                _buffPool.Release(buff);
            }
            else
            {
                buff.RemoveAfter += buffRemoved =>
                {
                    _buffPool.Release(buffRemoved);
                };
            }

            return buff;
        }

        public virtual bool CheckBuffHit(Damage damage)
        {
            return RandomUtil.rand.Next(100) < effectHitRate.Value;
        }

        private void OnDisable()
        {
            DMG.Reset();
            knockBackFactor.Reset();
            OnDealDamageFuncs.Clear();
            DealDamageAfter = null;
        }
    }

}

