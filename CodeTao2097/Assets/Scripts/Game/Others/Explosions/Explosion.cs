using System;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 爆炸单位，会在生成后爆炸，对周围单位造成伤害，并击退。
    /// </summary>
    public class Explosion : UnitController
    {
        protected Animator ani;
        public Damager damager;
        public BindableStat range = new BindableStat(2);

        public override void PreInit()
        {
            base.PreInit();
            ani = this.GetComponentInDescendants<Animator>();
            damager = GetComp<Damager>();
        }
        
        public override void Init()
        {
            base.Init();
            
            ActionKit.DelayFrame(1, Explode).Start(this);
            ActionKit.Delay(ani.GetCurrentAnimatorStateInfo(0).length, Deinit).Start(this);
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