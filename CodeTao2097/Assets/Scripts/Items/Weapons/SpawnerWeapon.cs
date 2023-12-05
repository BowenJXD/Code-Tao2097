using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace CodeTao
{
    public abstract class SpawnerWeapon<T> : Weapon where T : UnitController
    {
        protected BindableProperty<int> ammo = new BindableProperty<int>(3);
        public BindableProperty<int> maxAmmo = new BindableProperty<int>(3);
        public BindableProperty<float> reloadTime = new BindableProperty<float>(1);
        
        public T unitPrefab;
        protected UnitPool<T> pool;

        public BindableProperty<int> spawnAmount = new BindableProperty<int>(1);

        protected override void Start()
        {
            base.Start();
            
            ammo.Value = maxAmmo.Value;
            ammo.RegisterWithInitValue(value =>
            {
                if (value <= 0)
                {
                    StartReload();
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            pool = new UnitPool<T>(unitPrefab);
        }

        public override void Fire()
        {
            base.Fire();

            for (int i = 0; i < spawnAmount.Value; i++)
            {
                if (ammo.Value <= 0)
                {
                    break;
                }
                
                SpawnUnit(GetSpawnPoint(i));
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spawnPosition">The relative spawn position from transform</param>
        /// <returns></returns>
        public virtual T SpawnUnit(Vector2 spawnPosition)
        {
            T unit = pool.Get();
            unit.onDestroy = () =>
            {
                pool.Release(unit);
            };
            unit.transform.position = transform.position + (Vector3)spawnPosition;
            unit.transform.localScale = new Vector3(weaponSize, weaponSize);
            ammo.Value--;
            return unit;
        }

        public abstract Vector2 GetSpawnPoint(int spawnIndex);
        
        public virtual void StartReload()
        {
            fireLoop.Pause();
            ActionKit.Delay(reloadTime.Value, () =>
            {
                Reload();
            }).Start(this);
        }
        
        public virtual void Reload()
        {
            ammo.Value = maxAmmo.Value;
            fireLoop.Resume(true);
        }
    }
}