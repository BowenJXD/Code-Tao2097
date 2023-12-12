using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace CodeTao
{
    public abstract class SpawnerWeapon<T> : Weapon where T : UnitController
    {
        /// <summary>
        /// Reload time = cooldown / reloadCoolDownRatio
        /// </summary>
        public BindableProperty<float> coolDownReloadRatio = new BindableProperty<float>(0.5f);
        
        public T unitPrefab;
        protected UnitPool<T> pool;

        protected override void Start()
        {
            base.Start();
            
            pool = new UnitPool<T>(unitPrefab);
            
            ats[EWAt.Cooldown].RegisterWithInitValue(cooldown =>
            {
                reloadTime.Value = cooldown * coolDownReloadRatio.Value;
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public override void Fire()
        {
            base.Fire();

            for (int i = 0; i < ats[EWAt.Amount].Value; i++)
            {
                SpawnUnit(GetSpawnPoint(i));
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spawnPosition">The spawn position relative to the transform</param>
        /// <returns></returns>
        public virtual T SpawnUnit(Vector2 spawnPosition)
        {
            T unit = pool.Get();
            unit.onDestroy = () =>
            {
                pool.Release(unit);
            };
            unit.transform.position = transform.position + (Vector3)spawnPosition;
            return unit;
        }
        
        /// <summary>
        /// Get the spawn position relative to the transform
        /// </summary>
        /// <param name="spawnIndex">The index of the spawning item in one fire event</param>
        /// <returns></returns>
        public abstract Vector2 GetSpawnPoint(int spawnIndex);
    }
}