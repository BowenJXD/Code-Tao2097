﻿using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 生成单位的执行器。
    /// </summary>
    public class SpawnExecutor : WeaponExecutor
    {
        public UnitController unitPrefab;
        public bool rootToWeapon = false;
        protected Damager damager;

        public override void Init(Weapon newWeapon)
        {
            base.Init(newWeapon);
            if (!unitPrefab) { unitPrefab = this.GetComponentInDescendants<UnitController>(true); }
            UnitManager.Instance.Register(unitPrefab, rootToWeapon? transform : null);

            if (!damager) { damager = this.GetComponentInDescendants<Damager>(true); }

            if (damager)
            {
                if (damager.damageElementType == ElementType.None) { damager.damageElementType = weapon.ElementType; }

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
            UnitController unit = UnitManager.Instance.Get(unitPrefab);
            unit.Position(globalPos);

            if (unit is IWeaponDerivative weaponDerivative){
                weaponDerivative.SetWeapon(weapon, damager);
                weaponDerivative.InitSpawn(globalPos);
            }
            return unit;
        }
    }
}