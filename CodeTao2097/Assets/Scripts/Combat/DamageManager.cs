using System;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class DamageManager : MonoSingleton<DamageManager>
    {
        public Action<Damage> damageAfter;
        
        public Defencer ColToDef(Damager damager, Collider2D col)
        {
            UnitController unitController = ComponentUtil.GetComponentInAncestors<UnitController>(col);
            if (unitController)
            {
                Defencer defencer = ComponentUtil.GetComponentInDescendants<Defencer>(unitController);
                if (Util.IsTagIncluded(unitController.tag, damager.damagingTags) && defencer)
                {
                    return defencer;
                }
            }

            return null;
        }
        
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
            attacker?.DealDamageAfter?.Invoke(damage);
            damageAfter?.Invoke(damage);
            
            return damage;
        }
    }
}