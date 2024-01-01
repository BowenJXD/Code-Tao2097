using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class ResonanceWave : UnitController
    {
        protected SpriteRenderer sp;
        protected Collider2D col;
        protected Damager damager;
        public ResonanceWave SetDamager(Damager newDamager)
        {
            damager = newDamager;
            return this;
        }
        
        public float defaultPercent = 0.2f;
        public BindableStat range = new BindableStat(2);
        public BindableStat lifeTime = new BindableStat(1);

        protected List<Collider2D> collided = new List<Collider2D>();

        void Awake()
        {
            sp = this.GetComponentInDescendants<SpriteRenderer>();
            col = this.GetComponentInDescendants<Collider2D>();
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
            transform.DOScale(range, lifeTime).SetEase(Ease.InOutCirc). OnComplete(Finish);
        }

        void Attack(Collider2D col)
        {
            if (collided.Contains(col))
            {
                return;
            }
            collided.Add(col);
            Defencer def = DamageManager.Instance.ColToDef(damager, col);
            if (def)
            {
                DamageManager.Instance.ExecuteDamage(damager, def);
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
    }
}