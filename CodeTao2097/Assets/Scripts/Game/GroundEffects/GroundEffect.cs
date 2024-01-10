using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 攻击目标类型
    /// </summary>
    public enum EAttackTarget
    {
        None,
        One,
        All,
    }
    
    /// <summary>
    /// 领域GroundEffect：无实体，间歇性对领域内的敌人造成伤害。进出领域可以有伤害。
    /// </summary>
    public partial class GroundEffect : UnitController
    {
        [HideInInspector] public Damager damager;
        [HideInInspector] public Collider2D col2D;
        [HideInInspector] public Weapon weapon;
        
        public BindableStat attackInterval = new BindableStat(1f);
        public BindableStat lifeTime = new BindableStat(5f);
        
        public BindableProperty<EAttackTarget> attackWhenEntering = new BindableProperty<EAttackTarget>(EAttackTarget.None);
        public BindableProperty<EAttackTarget> attackWhenExiting = new BindableProperty<EAttackTarget>(EAttackTarget.None);

        public LoopTask attackLoop;

        private void Awake()
        {
            col2D = this.GetCollider();
        }

        protected virtual void OnEnable()
        {
            
           
        }

        public GroundEffect SetWeapon(Weapon newWeapon)
        {
            this.weapon = newWeapon;
            return this;
        }

        public override void Init()
        {
            base.Init();
            
            damager = weapon.damager;
            
            attackLoop = new LoopTask(this, attackInterval, AttackAll, Deinit);
            attackLoop.SetTimeCondition(lifeTime);
            attackLoop.Start();
            
            attackInterval.RegisterWithInitValue(interval =>
            {
                attackLoop.LoopInterval = interval;
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            lifeTime.RegisterWithInitValue(value =>
            {
                attackLoop.SetTimeCondition(value);
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            switch (attackWhenEntering.Value)
            {
                case EAttackTarget.None:
                    break;
                case EAttackTarget.One:
                    col2D.OnTriggerEnter2DEvent(col =>
                    {
                        Defencer target = DamageManager.Instance.ColToDef(damager, col);
                        if (target)
                        {
                            Attack(target);
                        }
                    }).UnRegisterWhenGameObjectDestroyed(this);
                    break;
                case EAttackTarget.All:
                    col2D.OnTriggerEnter2DEvent(col =>
                    {
                        Defencer target = DamageManager.Instance.ColToDef(damager, col);
                        if (target)
                        {
                            AttackAll();
                        }
                    }).UnRegisterWhenGameObjectDestroyed(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (attackWhenExiting.Value)
            {
                case EAttackTarget.None:
                    break;
                case EAttackTarget.One:
                    col2D.OnTriggerExit2DEvent(col =>
                    {
                        Defencer target = DamageManager.Instance.ColToDef(damager, col);
                        if (target)
                        {
                            Attack(target);
                        }
                    }).UnRegisterWhenGameObjectDestroyed(this);
                    break;
                case EAttackTarget.All:
                    col2D.OnTriggerExit2DEvent(col =>
                    {
                        Defencer target = DamageManager.Instance.ColToDef(damager, col);
                        if (target)
                        {
                            AttackAll();
                        }
                    }).UnRegisterWhenGameObjectDestroyed(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public virtual void AttackAll()
        {
            List<Collider2D> cols = new List<Collider2D>();
            Physics2D.OverlapCollider(col2D, new ContactFilter2D().NoFilter(), cols);
            
            foreach (var col in cols)
            {
                Defencer target = DamageManager.Instance.ColToDef(damager, col);
                if (target)
                {
                    Attack(target);
                }
            }
        }
        
        public virtual void Attack(Defencer defencer)
        {
            if (damager)
            {
                DamageManager.Instance.ExecuteDamage(damager, defencer, weapon? weapon.attacker : null);
            }
        }
    }
}