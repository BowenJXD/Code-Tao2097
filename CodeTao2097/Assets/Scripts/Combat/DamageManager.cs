using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 负责执行伤害事件，包括伤害的计算、触发、结算。
    /// </summary>
    public class DamageManager : MonoSingleton<DamageManager>
    {
        public Action<Damage> damageAfter;
        
        public Damage DamageCol(Damager damager, Collider2D col, Attacker attacker = null, AttackerUsage[] attackerUsages = null)
        {
            Defencer defencer = ColToDef(damager, col);
            if (defencer)
            {
                return ExecuteDamage(damager, defencer, attacker, attackerUsages);
            }
            return null;
        }
        
        public Defencer ColToDef(Damager damager, Collider2D col)
        {
            UnitController unitController = col.GetUnit();
            if (unitController && unitController.gameObject.activeSelf)
            {
                Defencer defencer = unitController.GetComp<Defencer>();
                if (Util.IsTagIncluded(unitController.tag, damager.damagingTags) && defencer)
                {
                    return defencer;
                }
            }

            return null;
        }

        public Damage ExecuteDamage(Damager damager, Defencer defencer, Attacker attacker = null, AttackerUsage[] attackerUsages = null)
        {
            Damage damage = ProcessDamage(damager, defencer, attacker, attackerUsages);
            if (damage == null)
            {
                return null;
            }

            return DealDamage(damage);
        }
        
        public Damage ProcessDamage(Damager damager, Defencer defencer, Attacker attacker = null, AttackerUsage[] attackerUsages = null)
        {
            if (!CheckDamage(damager, defencer, attacker))
            {
                return null;
            }
            if (attackerUsages == null)
            {
                attackerUsages = new [] { AttackerUsage.ATK, AttackerUsage.Crit, AttackerUsage.ElementBonus };
            }
            
            Damage damage = new Damage();
            attacker?.ProcessDamage(damage, attackerUsages);
            damager.ProcessDamage(damage);
            defencer.ProcessDamage(damage);
            
            attacker?.ProcessDamageExt(damage);
            damager.ProcessDamageExt(damage);
            defencer.ProcessDamageExt(damage);
            
            damage.CalculateDamageValue();
            return damage;
        }

        public bool CheckDamage(Damager damager, Defencer defencer, Attacker attacker = null)
        {
            bool result = true;
            result &= damager.ValidateDamage(defencer, attacker);
            result &= defencer.ValidateDamage(damager, attacker);
            return result;
        }

        public Damage DealDamage(Damage damage)
        {
            damage.Median.DealDamage(damage);
            damage.Source?.DealDamageAfter?.Invoke(damage);
            if (damage.Dealt)
            {
                damageAfter?.Invoke(damage);
            }
            return damage;
        }

        /*protected override void OnApplicationQuit()
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
                damagerStats[damager.name] += damage.CalculateDamageValue();
            }
            
            string log = "";
            foreach (var damagerStat in damagerStats)
            {
                log += $"{damagerStat.Key}: {damagerStat.Value}\n";
            }
            
            LogKit.I(log);
        }*/
    }
}