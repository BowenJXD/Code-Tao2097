using System.Collections.Generic;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 以生成单位为主要功能的武器的基类，需要指定生成地点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SpawnerWeapon<T> : Weapon where T : UnitController
    {
        public AnimateObject spawnAnimation;
        protected ObjectPool<AnimateObject> aniPool;
        public T unitPrefab;
        protected UnitPool<T> pool;

        public override void Init()
        {
            base.Init();

            if (!unitPrefab)
            {
                unitPrefab = this.GetComponentInDescendants<T>(true);
            }
            pool = new UnitPool<T>(unitPrefab);
            if (!spawnAnimation)
            {
                spawnAnimation = this.GetComponentInDescendants<AnimateObject>(true);
            }

            if (spawnAnimation){
                aniPool = new ObjectPool<AnimateObject>(
                    () => { return Instantiate(spawnAnimation, AnimateObjectManager.Instance.transform); }, 
                    prefab => { prefab.gameObject.SetActive(true); }, 
                    prefab => { prefab.gameObject.SetActive(false); }, 
                    prefab => { Destroy(prefab); });
            }
        }

        public override void Fire()
        {
            base.Fire();
            
            Vector2 baseDirection = GetBaseDirection();
            for (int i = 0; i < ats[EWAt.Amount].Value; i++)
            {
                Vector2 localSpawnPoint = GetLocalSpawnPoint(baseDirection, i);
                Vector2 globalSpawnPoint = transform.position + (Vector3)localSpawnPoint;
                if (aniPool != null)
                {
                    AnimateObject newAnim = aniPool.Get().Position(globalSpawnPoint);
                    newAnim.onTrigger = () => { SpawnUnit(globalSpawnPoint, localSpawnPoint).Init(); };
                    newAnim.onEnd = () => { aniPool.Release(newAnim); };
                }
                else
                {
                    SpawnUnit(globalSpawnPoint, localSpawnPoint).Init();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalPos"></param>
        /// <param name="localPos">The spawn position relative to the transform</param>
        /// <returns></returns>
        public virtual T SpawnUnit(Vector2 globalPos, Vector2 localPos)
        {
            T unit = pool.Get();
            unit.onDeinit = () =>
            {
                pool.Release(unit);
            };
            unit.Position(globalPos);
            return unit;
        }

        /// <summary>
        /// Get the spawn position relative to the transform
        /// </summary>
        /// <param name="basePoint">The base point to modify from</param>
        /// <param name="spawnIndex">The index of the spawning item in one fire event</param>
        /// <returns></returns>
        public abstract Vector2 GetLocalSpawnPoint(Vector2 basePoint, int spawnIndex);

        public virtual Vector2 GetBaseDirection(){ return Vector2.zero; }
    }
}