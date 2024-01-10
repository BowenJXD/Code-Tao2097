using UnityEngine;

namespace CodeTao.Buffs
{
    /// <summary>
    /// 痕印（弹射物增伤）Mark of Rebound（水）
    /// 附属者受到弹射物的伤害后，令其弹射物的伤害增加。
    /// </summary>
    public class MarkOfRebound : Buff
    {
        public float damageIncrement = 0.1f;
        public int maxStack = 5;
        private int _stack = 0;
        
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
        
        public Damage OnTakeDamage(Damage damage)
        {
            Projectile projectile = damage.Median.Unit as Projectile;
            if (projectile)
            {
                damage.SetDamageSection(DamageSection.DamageIncrement, name, damageIncrement * _stack, RepetitionBehavior.Overwrite);
                _stack = Mathf.Min(_stack + 1, maxStack);
            }
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