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
        public bool rootToParent = true;
        private Weapon weapon;
        private Damager damager;

        private void OnEnable()
        {
            if (!unitPrefab) unitPrefab = GetComponentInChildren<UnitController>();
            if (!damager) damager = GetComponentInChildren<Damager>();
            UnitManager.Instance.Register(unitPrefab, rootToParent? transform : null);
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
            UnitController unit = UnitManager.Instance.Get(unitPrefab);
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