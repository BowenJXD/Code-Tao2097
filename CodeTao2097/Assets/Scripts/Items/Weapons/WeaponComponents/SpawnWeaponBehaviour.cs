using System.Collections;
using System.Collections.Generic;
using QFramework;
using Schema.Builtin.Nodes;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 生成单位的执行器。
    /// </summary>
    public class SpawnWeaponBehaviour : WeaponBehaviour
    {
        public UnitController unitPrefab;
        public bool rootToWeapon = false;
        public bool useLocalPos = true;
        public bool waitUntilDeinit = false;
        protected Damager damager;

        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            if (!unitPrefab)
            {
                unitPrefab = this.GetComponentInDescendants<UnitController>(true);
            }
            UnitManager.Instance.Register(unitPrefab, rootToWeapon? transform : null);

            if (!damager) { damager = this.GetComponentInDescendants<Damager>(true); }

            if (damager)
            {
                if (damager.damageElementType == ElementType.None) { damager.damageElementType = weapon.ElementType; }
            }
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            for (int i = 0; i < localPositions.Count; i++)
            {
                Vector3 pos = useLocalPos ? transform.position + localPositions[i] : globalPositions[i];
                UnitController unit = SpawnUnit(pos);
                if (waitUntilDeinit)
                {
                    UnNext();
                    unit.onDeinit = Next;
                }
                unit.Init();
            }
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