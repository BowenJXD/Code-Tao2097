using System;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class Explosion : UnitController
    {
        protected Animation ani;
        public Damager damager;
        public BindableStat range = new BindableStat(2);

        public override void PreInit()
        {
            base.PreInit();
            ani = this.GetComponentInDescendants<Animation>();
            damager = this.GetComponentInDescendants<Damager>();
        }
        
        public override void Init()
        {
            base.Init();
            
            ActionKit.DelayFrame(5, Explode).Start(this);
            ActionKit.Delay(ani.clip.length, Deinit).Start(this);
        }

        void Explode()
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, range);
            foreach (Collider2D col in cols)
            {
                Attack(col);
            }
        }
        
        void Attack(Collider2D col)
        {
            Defencer def = DamageManager.Instance.ColToDef(damager, col);
            if (def)
            {
                DamageManager.Instance.ExecuteDamage(damager, def);
            }
        }
    }
}