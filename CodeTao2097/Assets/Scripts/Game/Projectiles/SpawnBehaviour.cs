using System;
using QFramework;
using UnityEngine.Serialization;

namespace CodeTao
{
    public class SpawnBehaviour : UnitBehaviour, IWAtReceiver
    {
        public BindableStat spawnInterval;
        LoopTask spawnTask;
        public UnitController unitPrefab;
        UnitPool<UnitController> pool;
        public bool rootToParent = true;
        private Weapon weapon;
        private Damager damager;

        private void OnEnable()
        {
            if (!unitPrefab) unitPrefab = GetComponentInChildren<UnitController>();
            if (!damager) damager = GetComponentInChildren<Damager>();
            if (pool == null) pool = new UnitPool<UnitController>(unitPrefab, rootToParent? transform : UnitManager.Instance.transform);
            if (!weapon){
                if (Unit is IWeaponDerivative weaponDerivative)
                {
                    weapon = weaponDerivative.weapon;
                }
            }
            spawnTask = new LoopTask(this, spawnInterval, Spawn);
            spawnInterval.RegisterWithInitValue( value => spawnTask.LoopInterval = value).
                UnRegisterWhenGameObjectDestroyed(this);
            spawnTask.Start();
        }

        void Spawn()
        {
            UnitController unit = pool.Get();
            unit.onDeinit = () =>
            {
                pool.Release(unit);
            };
            unit.Position(transform.position);

            if (unit is IWeaponDerivative weaponDerivative){
                weaponDerivative.SetWeapon(weapon, damager);
                weaponDerivative.InitSpawn(transform.position);
            }
            unit.Init();
        }

        private void OnDisable()
        {
            spawnTask?.Finish();
            spawnTask = null;
        }

        public void Receive(IWAtSource source)
        {
            spawnInterval.InheritStat(source.GetWAt(EWAt.Cooldown));
        }
    }
}