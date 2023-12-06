using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public class Weapon : Item
    {
        [SerializeField]
        public SerializableDictionary<EWAts, BindableStat> ats = new SerializableDictionary<EWAts, BindableStat>()
        {
            {EWAts.Damage, new BindableStat(10)}, 
            {EWAts.Amount, new BindableStat(1)},
            {EWAts.Duration, new BindableStat(5).SetMinValue(0.1f)},
            {EWAts.Speed, new BindableStat(4)},
            {EWAts.Cooldown, new BindableStat(2).SetMinValue(0.1f)},
            {EWAts.Area, new BindableStat(0)}
        };
        
        public BindableProperty<int> shotsToReload = new BindableProperty<int>(0);
        public BindableStat reloadTime = new BindableStat(0);
        public float attackRange => ats[EWAts.Area].Value > 0? ats[EWAts.Area].Value : 10;
        
        [HideInInspector] public Attacker attacker;
        
        [HideInInspector] public Damager damager;

        protected LoopTask fireLoop;

        protected override void Start()
        {
            base.Start();
            
            damager = ComponentUtil.GetComponentInDescendants<Damager>(this);
            ats[EWAts.Damage].RegisterWithInitValue(dmg =>
            {
                damager.DMG.AddModifier("weapon", dmg, EModifierType.Additive, ERepetitionBehavior.Overwrite);
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            // setup fire loop
            fireLoop = new LoopTask(this, ats[EWAts.Cooldown], Fire, StartReload);
            if (shotsToReload.Value > 0)
            {
                shotsToReload.RegisterWithInitValue(count =>
                {
                    fireLoop.SetCountCondition(count);
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
            fireLoop.Start();
            ats[EWAts.Cooldown].RegisterWithInitValue(interval =>
            {
                fireLoop.LoopInterval = interval;
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            LVL.RegisterWithInitValue(LevelUpAfter).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            // Add to inventory
            UnitController unitController = ComponentUtil.GetComponentInAncestors<UnitController>(this);
            if (unitController)
            {
                Container<Item> itemContainer = ComponentUtil.GetComponentInDescendants<Inventory>(unitController);
                if (itemContainer != null)
                {
                    ActionKit.DelayFrame(1, () =>
                    {
                        if (!this) return;
                        itemContainer.AddContent(this);
                    }).Start(this);
                }
            }
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

        public void LevelUpAfter(int newLvl)
        {
        }

        public virtual void Attack(Defencer defencer)
        {
            DamageManager.Instance.ExecuteDamage(damager, defencer, attacker);
        }
        
        
        /// <summary>
        /// Get available targets in attackRange in ascending order of distance
        /// </summary>
        /// <returns></returns>
        public List<Defencer> GetTargets(Collider2D colUsed = null)
        {
            List<Collider2D> colliders = new List<Collider2D>();
            if (colUsed)
            {
                Physics2D.OverlapCollider(colUsed, new ContactFilter2D().NoFilter(), colliders);
            }
            else
            {
                colliders = Physics2D.OverlapCircleAll(transform.position, attackRange).ToList();
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