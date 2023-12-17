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
        
        [BoxGroup("Secondary Attributes")]
        public BindableProperty<int> shotsToReload = new BindableProperty<int>(0);
        [BoxGroup("Secondary Attributes")]
        public BindableStat reloadTime = new BindableStat(0);
        [BoxGroup("Secondary Attributes")]
        [ShowInInspector] private BindableProperty<float> attackRange = new BindableProperty<float>(10);
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
            damager = ComponentUtil.GetComponentInDescendants<Damager>(this);
            damager.DamageElementType = elementType;
            ats[EWAt.Damage].RegisterWithInitValue(dmg =>
            {
                damager.DMG = ats[EWAt.Damage];
            }).UnRegisterWhenGameObjectDestroyed(this);
            
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
                fireLoop.LoopInterval = ats[EWAt.Cooldown];
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public virtual void Fire()
        {
            
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
                    ats[mod.attribute].AddModifier(mod.value, mod.modType, $"Level{newLevel}");
                    if (mod.exclusive) break;
                }
            }
        }
        
        public override string GetDescription()
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
            
            return base.GetDescription() + result.StringJoin("\n");
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