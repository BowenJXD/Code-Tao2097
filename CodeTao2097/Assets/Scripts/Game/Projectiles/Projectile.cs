using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public partial class Projectile : ViewController
    {
        [HideInInspector] public Rigidbody2D rigidbody2D;
        [HideInInspector] public Damager damager;
        [HideInInspector] public Collider2D col2D;
        [HideInInspector] public MoveController moveController;
        [HideInInspector] public Weapon weapon;
        public BindableProperty<float> lifeTime = new BindableProperty<float>(5f);
        public BindableProperty<int> penetration = new BindableProperty<int>(1);
        protected List<Collider2D> penetratedCols = new List<Collider2D>();

        protected virtual void Awake()
        {
            rigidbody2D = SelfRigidbody2D;
            col2D = PjtlCollider;
            moveController = MoveController;
            
            // change rigidbody2D's velocity when moveController's SPD or MovementDirection changed
            moveController.SPD.RegisterWithInitValue(value =>
            {
                rigidbody2D.velocity = moveController.MovementDirection.Value * value;
            }).UnRegisterWhenGameObjectDestroyed(this);
            moveController.MovementDirection.RegisterWithInitValue(value =>
            {
                rigidbody2D.velocity = moveController.SPD * value;
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            // attack when colliding with target
            col2D.OnTriggerEnter2DEvent(col =>
            {
                UnitController unitController = ComponentUtil.GetComponentInAncestors<UnitController>(col);
                if (unitController)
                {
                    Defencer defencer = ComponentUtil.GetComponentInDescendants<Defencer>(unitController);
                    if (Util.IsTagIncluded(unitController.tag, damager.damagingTags) && defencer)
                    {
                        Attack(defencer);
                        penetratedCols.Add(col);
                        if (penetration.Value <= penetratedCols.Count)
                        {
                            Destroy();
                        }
                    }
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            // destroy when lifeTime is over
            ActionKit.Delay(lifeTime.Value, () =>
            {
                Destroy();
            }).Start(this);
        }
        
        public virtual void Init(Weapon weapon, Vector2 direction)
        {
            this.weapon = weapon;
            damager = weapon.damager;
            moveController.MovementDirection.Value = direction;
            transform.rotation = Quaternion.Euler(0, 0, Util.GetAngleFromVector(direction));
        }
        
        public virtual void Attack(Defencer defencer)
        {
            if (damager)
            {
                DamageManager.Instance.ExecuteDamage(damager, defencer, weapon.attacker);
            }
        }
        
        public Action OnDestroy;
        
        public virtual void Destroy()
        {
            OnDestroy?.Invoke();
            OnDestroy = null;
        }
    }
}