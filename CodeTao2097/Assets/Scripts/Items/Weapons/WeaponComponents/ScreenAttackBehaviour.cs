using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 全屏攻击执行器，给予所有可见单位造成伤害。
    /// </summary>
    public class ScreenAttackBehaviour : WeaponBehaviour
    {
        private Damager damager;

        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            if (!damager) { damager = this.GetComponentInDescendants<Damager>(true); }
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            // get targets from all visible units on the screen
            List<Collider2D> targets = Util.GetVisibleColliders().ToList();
            
            foreach (var target in targets)
            {
                Defencer def = DamageManager.Instance.ColToDef(damager, target);
                if (def)
                {
                    DamageManager.Instance.ExecuteDamage(damager, def, weapon.attacker);
                }
            }
        }
    }
}