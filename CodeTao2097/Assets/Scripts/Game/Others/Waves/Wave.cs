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
    public class Wave : UnitController
    {
        public Ease easeMode = Ease.Linear;
        protected SpriteRenderer sp;
        protected Collider2D col;
        protected Damager damager;
        protected Attacker attacker;
        public Wave SetDamager(Damager newDamager)
        {
            damager = newDamager;
            return this;
        }

        public Wave SetWeapon(Weapon weapon)
        {
            damager = weapon.damager;
            attacker = weapon.attacker;
            return this;
        }
        
        public float defaultPercent = 0.2f;
        public BindableStat range = new BindableStat(2);
        public BindableStat lifeTime = new BindableStat(1);

        protected List<Collider2D> collided = new List<Collider2D>();

        void Awake()
        {
            sp = this.GetComponentInDescendants<SpriteRenderer>();
            col = this.GetCollider();
        }
        
        public override void Init()
        {
            base.Init();
            try
            {
                sp.material.SetFloat("_Percent", defaultPercent);
            }
            catch (Exception e)
            {
                LogKit.I(e);
            }

            col.OnTriggerEnter2DEvent(Attack).UnRegisterWhenGameObjectDestroyed(this);
            transform.localScale = Vector3.zero;
            transform.DOScale(range, lifeTime).SetEase(easeMode). OnComplete(Finish);
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
            /*sp.material.DOFloat(0, "_Percent", 0.2f).OnComplete(() =>
            {
                Deinit();
            });*/
            ActionKit.Delay(0.2f, Deinit).Start(this);
        }

        public override void Deinit()
        {
            base.Deinit();
            collided.Clear();
        }
    }
}