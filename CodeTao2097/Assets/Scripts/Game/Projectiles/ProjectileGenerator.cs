using QFramework;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace CodeTao
{
    public class ProjectilePool : ObjectPool<Projectile>
    {
        public ProjectilePool(Projectile defaultPrefab, int defaultCapacity = 10) :
            base(() =>
                {
                    Projectile instance = Object.Instantiate(defaultPrefab);
                    return instance;
                }, prefab =>
                {
                    prefab.gameObject.SetActive(true);
                }
                , prefab =>
                {
                    prefab.gameObject.SetActive(false);
                }
                , prefab => { Object.Destroy(prefab); }
                , true, defaultCapacity)
        {
        }
    }
    
    /*
    public class ProjectileGenerator : MonoSingleton<ExpGenerator>
    {
        public ProjectilePool pools;
        
        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            pools = new ProjectilePool(expBallPrefab);
        }

        public void GenerateProjectile(float expValue, Vector3 position)
        {
            ActionKit.DelayFrame(1, () =>
            {
                Projectile expBall = pools.Get();
                expBall.transform.position = position;
            }).Start(this);
        }
    }*/
}