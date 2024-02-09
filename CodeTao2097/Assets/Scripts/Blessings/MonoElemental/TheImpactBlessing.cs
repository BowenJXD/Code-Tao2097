using System.Linq;
using UnityEngine;

namespace CodeTao
{
    public class TheImpactBlessing : Blessing
    {
        public float knockbackMultiplier = 0.1f;
        public float maxMultiplier = 0.5f;
        private Attacker attacker;
        
        public override void Init()
        {
            base.Init();
            attacker = Container.GetComp<Attacker>();
            attacker.onDealDamageFuncs.Add(OnDealDamage);
        }
        
        Damage OnDealDamage(Damage damage)
        {
            if (damage.DamageElement == relatedElements.FirstOrDefault())
            {
                if (damage.Target.GetComp<BuffOwner>().FindAll(buff => buff is PetrifiedBuff).Count > 0)
                {
                    float value = damage.Knockback * knockbackMultiplier;
                    value = Mathf.Clamp(value, 0, maxMultiplier);
                    damage.SetDamageSection(DamageSection.DamageIncrement, name, 1 + value);
                }
            }

            return damage;
        }
    }
}