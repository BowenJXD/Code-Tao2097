using QFramework;

namespace CodeTao
{
    public class DamageManager : MonoSingleton<DamageManager>
    {
        public bool CheckDamage(Damager damager, Defencer defencer, Attacker attacker = null)
        {
            bool result = true;
            result &= damager.ValidateDamage(defencer, attacker);
            result &= defencer.ValidateDamage(damager, attacker);
            return result;
        }
        
        public Damage ExecuteDamage(Damager damager, Defencer defencer, Attacker attacker = null)
        {
            if (!CheckDamage(damager, defencer, attacker))
            {
                return null;
            }
            
            damager.StartCD();
            defencer.StartCD();
            
            Damage damage = new Damage();
            attacker?.ProcessDamage(damage);
            damager.ProcessDamage(damage);
            defencer.ProcessDamage(damage);
            
            damager.DealDamage(damage);
            return damage;
        }
    }
}