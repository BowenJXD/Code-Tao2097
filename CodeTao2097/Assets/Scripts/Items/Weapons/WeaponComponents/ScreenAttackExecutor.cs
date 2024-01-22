using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 全屏攻击执行器，给予所有可见单位造成伤害。
    /// </summary>
    public class ScreenAttackExecutor : WeaponExecutor
    {
        private Damager damager;

        public override void Init(Weapon newWeapon)
        {
            base.Init(newWeapon);
            if (!damager) { damager = this.GetComponentInDescendants<Damager>(true); }
        }

        public override void Execute(List<Vector3> globalPositions)
        {
            base.Execute(globalPositions);
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