using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public class Weapon : Item
    {
        public ElementType elementType = ElementType.None;
        
        [SerializeField]
        public SerializableDictionary<EWAt, BindableStat> ats = new SerializableDictionary<EWAt, BindableStat>()
        {
            {EWAt.Damage, new BindableStat(10)}, 
            {EWAt.Amount, new BindableStat(1)},
            {EWAt.Duration, new BindableStat(5).SetMinValue(0.1f)},
            {EWAt.Speed, new BindableStat(4)},
            {EWAt.Cooldown, new BindableStat(2).SetMinValue(0.1f)},
            {EWAt.Area, new BindableStat(1).SetMinValue(0.1f)}
        };

        public Buff buffToApply;
        /// <summary>
        /// 0 - 100
        /// </summary>
        public BindableStat buffHitRate = new BindableStat(50);
        
        public BindableProperty<int> shotsToReload = new BindableProperty<int>(0);
        public BindableStat reloadTime = new BindableStat(0);
        [ShowInInspector] public BindableStat attackRange = new BindableStat(10);
        public virtual float AttackRange => attackRange.Value;
        
        [HideInInspector] public Attacker attacker;
        [HideInInspector] public Damager damager;

        protected LoopTask fireLoop;
        
        public List<WeaponUpgradeMod> upgradeMods = new List<WeaponUpgradeMod>();

        public override void OnAdd()
        {
            base.OnAdd();
            attacker = ComponentUtil.GetComponentFromUnit<Attacker>(Container);
        }

        public override void Init()
        {
            base.Init();
            damager = this.GetComponentInDescendants<Damager>();
            damager.DamageElementType = elementType;
            damager.DMG = ats[EWAt.Damage];
            damager.DealDamageAfter += TryApplyBuff;
            buffToApply = this.GetComponentInDescendants<Buff>();
            
            // setup fire loop
            fireLoop = new LoopTask(this, ats[EWAt.Cooldown], Fire, StartReload);
            if (shotsToReload.Value > 0)
            {
                shotsToReload.RegisterWithInitValue(count =>
                {
                    fireLoop.SetCountCondition(count);
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
            fireLoop.Start();
            ats[EWAt.Cooldown].RegisterWithInitValue(interval =>
            {
                fireLoop.LoopInterval = interval;
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            _buffPool = new ContentPool<Buff>(buffToApply);
        }

        public virtual void Fire()
        {
            
        }
        
        private ContentPool<Buff> _buffPool;
        
        public virtual void TryApplyBuff(Damage damage)
        {
            if (damage.Target.IsDead) return;
            BuffOwner target = ComponentUtil.GetComponentFromUnit<BuffOwner>(damage.Target);
            if (target && CheckBuffHit(damage))
            {
                ApplyBuff(target);
            }
        }
        
        public virtual Buff ApplyBuff(BuffOwner target)
        {
            Buff buff = _buffPool.Get().Parent(this);
            buff.duration.AddModifierGroups(ats[EWAt.Duration].ModGroups);
            
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
            return RandomUtil.rand.Next(100) < buffHitRate.Value;
        }
        
        public virtual void StartReload()
        {
            ActionKit.Delay(reloadTime.Value, () =>
            {
                Reload();
            }).Start(this);
        }
        
        public virtual void Reload()
        {
            fireLoop.Start();
        }

        public override void Upgrade(int lvlIncrement = 1)
        {
            base.Upgrade(lvlIncrement);
            int newLevel = LVL.Value;

            foreach (var mod in upgradeMods)
            {
                if (mod.CheckCondition(newLevel))
                {
                    ModAttribute(mod);
                    if (mod.exclusive) break;
                }
            }
        }
        
        public virtual void ModAttribute(WeaponUpgradeMod mod)
        {
            if (ats.Contains(mod.attribute))
            {
                ats[mod.attribute].AddModifier(mod.value, mod.modType, $"Level{LVL + 1}");
            }
            else if (mod.attribute == EWAt.Range)
            {
                attackRange.AddModifier(mod.value, mod.modType, $"Level{LVL + 1}");
            }
            else if (mod.attribute == EWAt.Knockback)
            {
                damager.KnockBackFactor.AddModifier(mod.value, mod.modType, $"Level{LVL + 1}");
            }
        }
        
        public override string GetUpgradeDescription()
        {
            List<string> result = new List<string>();
            int newLevel = LVL.Value + 1;
            
            foreach (var mod in upgradeMods)
            {
                if (mod.CheckCondition(newLevel))
                {
                    result.Add(mod.GetDescription());
                    if (mod.exclusive) break;
                }
            }
            
            return base.GetUpgradeDescription() + result.StringJoin("\n");
        }

        public virtual void Attack(Defencer defencer)
        {
            Damage dmg = DamageManager.Instance.ExecuteDamage(damager, defencer, attacker);
        }
        
        
        /// <summary>
        /// Get available targets in attackRange in ascending order of distance
        /// </summary>
        /// /// <param name="range">default to be attackRange</param>
        /// <param name="colUsed"></param>
        /// <returns>Sorted in ascending order of distance to the ordering position.</returns>
        public List<Defencer> GetTargets(float range = 0, Collider2D colUsed = null)
        {
            List<Collider2D> colliders = new List<Collider2D>();
            if (range == 0)
            {
                range = AttackRange;
            }
            if (colUsed)
            {
                Physics2D.OverlapCollider(colUsed, new ContactFilter2D().NoFilter(), colliders);
            }
            else
            {
                colliders = Physics2D.OverlapCircleAll(transform.position, range).ToList();
            }

            SortedList<float, Defencer> targetDistances = new SortedList<float, Defencer>();
            foreach (var col in colliders)
            {
                Defencer target = DamageManager.Instance.ColToDef(damager, col);
                if (target)
                {
                    float targetDistance = Vector2.Distance(transform.position, col.transform.position);
                    targetDistances.Add(targetDistance, target);
                }
            }

            return targetDistances.Values.ToList();
        }
    }
}