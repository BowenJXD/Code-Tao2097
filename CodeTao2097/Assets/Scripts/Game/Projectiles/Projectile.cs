using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public partial class Projectile : UnitController
    {
        [HideInInspector] public Rigidbody2D rb2D;
        [HideInInspector] public Collider2D col2D;
        [HideInInspector] public MoveController moveController;
        [HideInInspector] public Weapon weapon;
        [HideInInspector] public Damager damager;
        public BindableStat lifeTime = new BindableStat(5f);
        private LoopTask _lifeTimeTask;
        
        /// <summary>
        /// The number of individual targets that the projectile can penetrate, when reached, the projectile will be destroyed.
        /// If <= 0, the projectile will not be destroyed.
        /// </summary>
        public BindableStat penetration = new BindableStat(1);
        protected List<Collider2D> penetratedCols = new List<Collider2D>();

        public Projectile SetMovingDirection(Vector2 direction)
        {
            moveController.MovementDirection.Value = direction;
            return this;
        }
        
        public Projectile SetWeapon(Weapon newWeapon)
        {
            this.weapon = newWeapon;
            damager = weapon.damager;
            return this;
        }
        
        public Projectile SetPenetration(BindableStat newPenetration)
        {
            penetration = newPenetration;
            return this;
        }
        
        public Projectile SetLifeTime(BindableStat newLifeTime)
        {
            lifeTime = newLifeTime;
            return this;
        }
        
        public Projectile SetSPD(BindableStat newSPD)
        {
            moveController.SPD = newSPD;
            return this;
        }

        public override void PreInit()
        {
            base.PreInit();
            
            rb2D = GetComponent<Rigidbody2D>();
            col2D = this.GetComponentInDescendants<Collider2D>();
            moveController = this.GetComponentInDescendants<MoveController>();
        }

        public override void Init()
        {
            base.Init();
            
            // destroy when lifeTime is over
            _lifeTimeTask = new LoopTask(this, lifeTime.Value, Deinit);
            _lifeTimeTask.SetCountCondition(1);
            _lifeTimeTask.Start();

            if (rb2D)
            {
                // change rigidbody2D's velocity when moveController's SPD or MovementDirection changed
                moveController.SPD.RegisterWithInitValue(value =>
                {
                    rb2D.velocity = moveController.MovementDirection.Value * value;
                }).UnRegisterWhenGameObjectDestroyed(this);
                moveController.MovementDirection.RegisterWithInitValue(value =>
                {
                    rb2D.velocity = moveController.SPD * value;
                }).UnRegisterWhenGameObjectDestroyed(this);
            }

            // attack when colliding with target
            col2D.OnTriggerEnter2DEvent(col =>
            {
                Defencer target = DamageManager.Instance.ColToDef(damager, col);
                if (target)
                {
                    Attack(target);
                    Penetrate(col);
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
        }
        
        public virtual void Attack(Defencer defencer)
        {
            if (damager)
            {
                DamageManager.Instance.ExecuteDamage(damager, defencer, weapon? weapon.attacker : null);
            }
        }

        public void Penetrate(Collider2D col)
        {
            if (penetration <= 0)
            {
                return;
            }
            penetratedCols.Add(col);
            if (penetration <= penetratedCols.Count)
            {
                Deinit();
            }
        }
        
        public override void Deinit()
        {
            base.Deinit();
            _lifeTimeTask.Pause();
        }
    }
}