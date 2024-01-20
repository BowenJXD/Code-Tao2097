using System;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 从自身出发，呈圆形向外扩散的单位，通过触碰来造成伤害。
    /// </summary>
    public class Wave : UnitController, IWeaponDerivative
    {
        public Ease easeMode = Ease.Linear;
        protected SpriteRenderer sp;
        protected Damager damager;
        protected Attacker attacker;
        public Wave SetDamager(Damager newDamager)
        {
            damager = newDamager;
            return this;
        }

        public Weapon weapon { get; set; }
        public virtual void SetWeapon(Weapon newWeapon, Damager newDamager)
        {
            weapon = newWeapon;
            if (!damager) damager = newDamager;
            attacker = weapon.attacker;
        }

        public float defaultPercent = 0.2f;

        protected List<Collider2D> collided = new List<Collider2D>();

        public override void PreInit()
        {
            base.PreInit();
            if (!sp) sp = this.GetComponentInDescendants<SpriteRenderer>();
        }
        
        public override void Init()
        {
            base.Init();
            if (sp.material.HasProperty("_Percent"))
            {
                sp.material.SetFloat("_Percent", defaultPercent);
            }

            this.GetCollider()?.OnTriggerEnter2DEvent(Attack).UnRegisterWhenGameObjectDestroyed(this);
            GetComp<LoopTaskController>()?.AddFinish(Finish);
            transform.localScale = Vector3.zero;
            transform.DOScale(1, GetComp<LoopTaskController>()?.duration).SetEase(easeMode);
        }

        protected virtual void Attack(Collider2D col)
        {
            if (collided.Contains(col))
            {
                return;
            }
            collided.Add(col);
            Defencer def = DamageManager.Instance.ColToDef(damager, col);
            if (def)
            {
                DamageManager.Instance.ExecuteDamage(damager, def, attacker);
            }
        }
        
        void Finish()
        {
            if (sp.material.HasProperty("_Percent"))
            {
                sp.material.DOFloat(0, "_Percent", 0.2f).OnComplete(Deinit);
            }
            else
            {
                ActionKit.Delay(0.2f, Deinit).Start(this);
            }
        }

        public override void Deinit()
        {
            base.Deinit();
            collided.Clear();
        }
    }
}