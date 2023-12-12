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
            Dictionary<Damager, float> damagerStats = new Dictionary<Damager, float>();
            foreach (var damage in _damagesLog)
            {
                Damager damager = damage.Median;
                if (ComponentUtil.GetUnitController(damager) != Player.Instance)
                {
                    continue;
                }
                
                if (!damagerStats.ContainsKey(damager))
                {
                    damagerStats.Add(damager, 0);
                }
                damagerStats[damager] += damage.GetDamageValue();
            }
            
            string log = "";
            foreach (var damagerStat in damagerStats)
            {
                log += $"{damagerStat.Key.name}: {damagerStat.Value}\n";
            }
            
            LogKit.I(log);
        }
    }
}