using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class DamageManager : MonoSingleton<DamageManager>
    {
        private List<Damage> _damagesLog = new List<Damage>();
        
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
            if (damage.Dealt)
            {
                damageAfter?.Invoke(damage);
                _damagesLog.Add(damage);
            }

            return damage;
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            LogDamageStat();
        }

        public void LogDamageStat()
        {
            Dictionary<string, float> damagerStats = new Dictionary<string, float>();
            foreach (var damage in _damagesLog)
            {
                Damager damager = damage.Median;
                
                if (!damagerStats.ContainsKey(damager.name))
                {
                    damagerStats.Add(damager.name, 0);
                }
                damagerStats[damager.name] += damage.GetDamageValue();
            }
            
            string log = "";
            foreach (var damagerStat in damagerStats)
            {
                log += $"{damagerStat.Key}: {damagerStat.Value}\n";
            }
            
            LogKit.I(log);
        }
    }
}