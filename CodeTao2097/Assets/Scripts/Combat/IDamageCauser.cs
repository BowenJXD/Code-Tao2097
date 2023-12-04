using System;

namespace CodeTao
{
    public interface IDamageCauser
    {
        public virtual void Attack(Damager damager, Defencer defencer, Attacker attacker = null)
        {
            if (damager && defencer)
            {
                DamageManager.Instance.ExecuteDamage(damager, defencer, attacker);
            }
        }
        
        
    }
}