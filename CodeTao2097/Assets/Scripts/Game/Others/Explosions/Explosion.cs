using System;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 爆炸单位，会在生成后爆炸，对周围单位造成伤害，并击退。
    /// 爆炸的damager的dmg和knockbackfactor由全局控制，damageelement和dealDamageAfter由weapon控制。
    /// </summary>
    public class Explosion : UnitController, IWeaponDerivative, IWAtReceiver
    {
        protected Animator ani;
        public Damager damager;
        public BindableStat area = new BindableStat(2);

        public override void PreInit()
        {
            base.PreInit();
            if (!ani) ani = this.GetComponentInDescendants<Animator>();
        }
        
        public override void Init()
        {
            base.Init();
            
            ActionKit.DelayFrame(1, Explode).Start(this);
            ActionKit.Delay(ani.GetCurrentAnimatorStateInfo(0).length, Deinit).Start(this);
        }

        void Explode()
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, area);
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

        public Weapon weapon { get; set; }
        public void SetWeapon(Weapon newWeapon, Damager newDamager)
        {
            weapon = newWeapon;
            if (!damager) damager = newDamager;
            damager.AddDamageTag(DamageTag.Explosion);
            damager = ExplosionGenerator.Instance.ModDamager(damager);
            damager.damageElementType = weapon.ElementType;
        }

        public void Receive(IWAtSource source)
        {
            area.InheritStat(source.GetWAt(EWAt.Area));
        }
    }
}