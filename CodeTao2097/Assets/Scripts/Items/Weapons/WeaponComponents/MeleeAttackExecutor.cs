using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    public class MeleeAttackExecutor : WeaponExecutor
    {
        public BindableStat angularRange = new BindableStat(0).SetMaxValue(360f);
        public BindableStat attackRange = new BindableStat(0);
        protected Damager damager;

        public override void Init(Weapon newWeapon)
        {
            base.Init(newWeapon);
            angularRange.AddModifierGroups(weapon.area.ModGroups);
            attackRange = weapon.attackRange;
            attackRange.AddModifierGroups(weapon.area.ModGroups);
            if (!damager) { damager = this.GetComponentInDescendants<Damager>(true); }
        }

        public override void Execute(List<Vector3> globalPositions)
        {
            base.Execute(globalPositions);
            List<float> attackingAngles = new List<float>();
            for (int i = 0; i < globalPositions.Count; i++)
            {
                Vector3 localPosition = globalPositions[i] - transform.position;
                float spawnAngle = Util.GetAngleFromVector(localPosition);
                attackingAngles.Add(spawnAngle);
            }
            
            List<Defencer> targets = weapon.GetTargets(attackRange);
            foreach (var target in targets)
            {
                float targetAngle = Util.GetAngleFromVector(target.transform.position - transform.position);
                foreach (var attackingAngle in attackingAngles)
                {
                    if (Mathf.Abs(attackingAngle - targetAngle) < angularRange / 2)
                    {
                        Attack(target);
                    }
                }
            }
        }
        
        public void Attack(Defencer defencer)
        {
            Damage dmg = DamageManager.Instance.ExecuteDamage(damager, defencer, weapon.attacker);
        }
    }
}