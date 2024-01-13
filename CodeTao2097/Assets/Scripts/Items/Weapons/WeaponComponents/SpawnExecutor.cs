using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class SpawnExecutor : WeaponExecutor
    {
        public UnitController unitPrefab;
        protected UnitPool<UnitController> pool;
        public bool rootToWeapon = false;
        protected Damager damager;

        public override void Init(Weapon newWeapon)
        {
            base.Init(newWeapon);
            if (!unitPrefab) { unitPrefab = this.GetComponentInDescendants<UnitController>(true); }
            pool = new UnitPool<UnitController>(unitPrefab);

            if (!damager) { damager = this.GetComponentInDescendants<Damager>(true); }

            if (damager)
            {
                if (damager.damageElementType == ElementType.None) { damager.damageElementType = weapon.elementType; }

                damager.DMG.InheritStat(weapon.damage);
                damager.knockBackFactor.InheritStat(weapon.knockBack);
                damager.effectHitRate.InheritStat(weapon.effectHitRate);
                damager.effectDuration.InheritStat(weapon.duration);
            }
            
        }

        public override void Execute(List<Vector3> globalPositions)
        {
            base.Execute(globalPositions);
            for (int i = 0; i < globalPositions.Count; i++)
            {
                SpawnUnit(globalPositions[i]).Init();
            }
            
            Next();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalPos"></param>
        /// <param name="localPos">The spawn position relative to the transform</param>
        /// <returns></returns>
        public virtual UnitController SpawnUnit(Vector2 globalPos)
        {
            UnitController unit = pool.Get();
            unit.onDeinit = () =>
            {
                pool.Release(unit);
            };
            unit.Position(globalPos);
            unit.Parent(rootToWeapon ? transform : UnitManager.Instance.GetTransform<UnitController>());

            if (unit is IWeaponDerivative weaponDerivative){
                weaponDerivative.SetWeapon(weapon, damager);
                weaponDerivative.InitSpawn(globalPos);
            }
            return unit;
        }
    }
}