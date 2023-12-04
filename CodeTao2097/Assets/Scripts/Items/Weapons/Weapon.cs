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
        public BindableStat attackInterval = new BindableStat(2).SetMinValue(0.1f);
        
        public BindableStat attackRange = new BindableStat(1);

        [HideInInspector] public Attacker attacker;
        
        [HideInInspector] public Damager damager;

        protected LoopTask fireLoop;

        protected virtual void Start()
        {
            fireLoop = new LoopTask(this, attackInterval, Fire);
            fireLoop.Start();
            attackInterval.RegisterWithInitValue(interval =>
            {
                fireLoop.LoopInterval = interval;
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            // Add to inventory
            UnitController unitController = ComponentUtil.GetComponentInAncestors<UnitController>(this);
            if (unitController)
            {
                IContainer<Item> itemContainer = ComponentUtil.GetComponentInDescendants<Inventory>(unitController);
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
        
        public virtual void Attack(Defencer defencer)
        {
            DamageManager.Instance.ExecuteDamage(damager, defencer, attacker);
        }
        
        
        /// <summary>
        /// Get available targets in attackRange in ascending order of distance
        /// </summary>
        /// <returns></returns>
        public List<Defencer> GetTargets()
        {
            List<Collider2D> colliders = Physics2D.OverlapCircleAll(transform.position, attackRange).ToList();
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