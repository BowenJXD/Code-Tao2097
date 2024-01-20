using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 在拥有者受到特定伤害标签的伤害时触发的buff
    /// </summary>
    public abstract class DamageTagBuff : Buff
    {
        public DamageTag damageTag;
        
        protected Defencer defencer;
        
        public override void OnAdd()
        {
            base.OnAdd();
            defencer = Container.GetComp<Defencer>();
            
            if (defencer)
            {
                defencer.OnTakeDamageFuncs.Add(OnTakeDamage);
            }
        }
        
        public virtual Damage OnTakeDamage(Damage damage)
        {
            if (damage.HasDamageTag(damageTag))
            {
                return ProcessDamage(damage);
            }
            return damage;
        }
        
        protected virtual Damage ProcessDamage(Damage damage)
        {
            return damage;
        }

        public override void OnRemove()
        {
            base.OnRemove();
            if (defencer)
            {
                defencer.OnTakeDamageFuncs.Remove(OnTakeDamage);
            }
        }
    }
}