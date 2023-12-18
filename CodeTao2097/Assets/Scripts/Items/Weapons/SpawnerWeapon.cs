using System.Collections.Generic;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace CodeTao
{
    public abstract class SpawnerWeapon<T> : Weapon where T : UnitController
    {
        public T unitPrefab;
        protected UnitPool<T> pool;

        /// <summary>
        /// Reload time = cooldown / reloadCoolDownRatio
        /// </summary>
        [BoxGroup("Secondary Attributes")]
        public BindableProperty<float> coolDownReloadRatio = new BindableProperty<float>(0.5f);

        public override void Init()
        {
            base.Init();

            if (!unitPrefab)
            {
                unitPrefab = ComponentUtil.GetComponentInDescendants<T>(this);
            }
            pool = new UnitPool<T>(unitPrefab);
            
            ats[EWAt.Cooldown].RegisterWithInitValue(cooldown =>
            {
                reloadTime.Value = cooldown * coolDownReloadRatio.Value;
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public override void Fire()
        {
            base.Fire();
            
            Vector2 baseDirection = GetBaseDirection();
            for (int i = 0; i < ats[EWAt.Amount].Value; i++)
            {
                Vector2 spawnPoint = GetSpawnPoint(baseDirection, i);
                SpawnUnit(spawnPoint);
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
            unit.onDeinit = () =>
            {
                pool.Release(unit);
            };
            unit.Position(transform.position + (Vector3)spawnPosition);
            return unit;
        }

        /// <summary>
        /// Get the spawn position relative to the transform
        /// </summary>
        /// <param name="basePoint">The base point to modify from</param>
        /// <param name="spawnIndex">The index of the spawning item in one fire event</param>
        /// <returns></returns>
        public abstract Vector2 GetSpawnPoint(Vector2 basePoint, int spawnIndex);

        public virtual Vector2 GetBaseDirection(){ return Vector2.zero; }
    }
}