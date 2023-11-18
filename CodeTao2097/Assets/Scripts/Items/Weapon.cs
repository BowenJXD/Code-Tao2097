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

        protected LoopTask FireLoop;

        protected virtual void Start()
        {
            FireLoop = new LoopTask(this, attackInterval, Fire);
            FireLoop.Start();
            attackInterval.RegisterWithInitValue(interval =>
            {
                FireLoop.LoopInterval = interval;
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public virtual void Fire()
        {
            
        }
        
        public virtual void Attack(Defencer defencer)
        {
            DamageManager.Instance.ExecuteDamage(damager, defencer, attacker);
        }
    }
}