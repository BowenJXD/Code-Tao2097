using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// Modify damage taken by a specific tag.
    /// </summary>
    public class DamageTagBuff : Buff
    {
        public DamageTag damageTag;
        public float damageIncrement = 0.1f;
        public bool upgradeOnTakeDamage = false;
        
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
            if (damage.HasDamageTag(damageTag))
            {
                damage.SetDamageSection(DamageSection.DamageIncrement, name, damageIncrement * LVL, RepetitionBehavior.Overwrite);
                if (upgradeOnTakeDamage) Upgrade();
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