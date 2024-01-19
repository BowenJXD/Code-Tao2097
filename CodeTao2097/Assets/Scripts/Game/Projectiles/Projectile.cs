using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 弹射物Projectile：无实体。触碰到单位后造成伤害。
    /// </summary>
    public partial class Projectile : UnitController, IWeaponDerivative, IWAtReceiver
    {
        [HideInInspector] public Rigidbody2D rb2D;
        [HideInInspector] public Collider2D col2D;
        [HideInInspector] public MoveController moveController;
        [HideInInspector] public Damager damager;
        public BindableStat lifeTime = new BindableStat(5f);
        public BindableStat area = new BindableStat(1f);
        private LoopTask _lifeTimeTask;
        
        /// <summary>
        /// The number of individual targets that the projectile can penetrate, when reached, the projectile will be destroyed.
        /// If <= 0, the projectile will not be destroyed.
        /// </summary>
        public BindableStat penetration = new BindableStat(1);
        protected List<Collider2D> penetratedCols = new List<Collider2D>();

        public Weapon weapon { get; set; }
        void IWeaponDerivative.SetWeapon(Weapon newWeapon, Damager newDamager)
        {
            weapon = newWeapon;
            if (!damager) damager = newDamager;
            damager.AddDamageTag(DamageTag.Projectile);
        }

        public void InitSpawn(Vector3 globalPos)
        {
            Vector3 localPos = globalPos - weapon.transform.position;
            this.Rotation(Quaternion.Euler(0, 0, Util.GetAngleFromVector(localPos.normalized)));
            moveController.movementDirection.Value = localPos.normalized;
        }

        public override void PreInit()
        {
            base.PreInit();
            
            rb2D = GetComponent<Rigidbody2D>();
            col2D = this.GetCollider();
            moveController = GetComp<MoveController>();
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
                    rb2D.velocity = moveController.movementDirection.Value * value;
                }).UnRegisterWhenGameObjectDestroyed(this);
                moveController.movementDirection.RegisterWithInitValue(value =>
                {
                    rb2D.velocity = moveController.SPD * value;
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
            
            area.RegisterWithInitValue(value =>
            {
                this.LocalScale(new Vector3(value, value));
            }).UnRegisterWhenGameObjectDestroyed(this);
            
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

        public void Receive(IWAtSource source)
        {
            area.InheritStat(source.GetWAt(EWAt.Area));
            lifeTime.InheritStat(source.GetWAt(EWAt.Duration));
            moveController.SPD.InheritStat(source.GetWAt(EWAt.Speed));
        }
    }
}