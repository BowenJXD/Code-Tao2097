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
        [HideInInspector] public Damager damager;
        [HideInInspector] public Collider2D col2D;
        [HideInInspector] public MoveController moveController;
        [HideInInspector] public Weapon weapon;
        public BindableProperty<float> lifeTime = new BindableProperty<float>(5f);
        private LoopTask _lifeTimeTask;
        public BindableProperty<int> penetration = new BindableProperty<int>(1);
        protected List<Collider2D> penetratedCols = new List<Collider2D>();

        protected virtual void Awake()
        {
            rb2D = SelfRigidbody2D;
            col2D = PjtlCollider;
            moveController = MoveController;
        }
        
        public virtual void Init(Weapon weapon, Vector2 direction)
        {
            this.weapon = weapon;
            damager = weapon.damager;
            moveController.MovementDirection.Value = direction;
            transform.rotation = Quaternion.Euler(0, 0, Util.GetAngleFromVector(direction));
            
            // destroy when lifeTime is over
            // ActionKit.Delay(lifeTime.Value, Destroy).Start(this);
            _lifeTimeTask = new LoopTask(this, lifeTime.Value, Destroy);
            _lifeTimeTask.SetCountCondition(1);
            _lifeTimeTask.Start();

            // change rigidbody2D's velocity when moveController's SPD or MovementDirection changed
            moveController.SPD.RegisterWithInitValue(value =>
            {
                rb2D.velocity = moveController.MovementDirection.Value * value;
            }).UnRegisterWhenGameObjectDestroyed(this);
            moveController.MovementDirection.RegisterWithInitValue(value =>
            {
                rb2D.velocity = moveController.SPD * value;
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            // attack when colliding with target
            col2D.OnTriggerEnter2DEvent(col =>
            {
                Defencer target = DamageManager.Instance.ColToDef(damager, col);
                if (target)
                {
                    Attack(target);
                    penetratedCols.Add(col);
                    if (penetration.Value <= penetratedCols.Count)
                    {
                        Destroy();
                    }
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

        public virtual void Destroy()
        {
            onDestroy?.Invoke();
            onDestroy = null;
            _lifeTimeTask.Pause();
        }
    }
}