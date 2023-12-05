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
    
    public partial class ProjectileManager : MonoSingleton<ProjectileManager>
    {
        
    }
}