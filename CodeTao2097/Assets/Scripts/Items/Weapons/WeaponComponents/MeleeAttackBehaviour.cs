using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    ///  通过physics2D对指定角度范围的单位造成近战伤害。
    /// </summary>
    public class MeleeAttackBehaviour : WeaponBehaviour, IWAtReceiver
    {
        public BindableStat angularRange = new BindableStat(0).SetMaxValue(360f);
        public BindableStat attackRange = new BindableStat(0);
        protected Damager damager;

        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            if (!damager) { damager = this.GetComponentInDescendants<Damager>(true); }
            if (damager) damager.AddDamageTag(DamageTag.Melee);
        }

        protected override void OnExecute()
        {
            base.OnExecute();
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

        public void Receive(IWAtSource source)
        {
            angularRange.InheritStat(source.GetWAt(EWAt.Area));
            attackRange.InheritStat(source.GetWAt(EWAt.Area));
        }
    }
}