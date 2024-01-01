using QFramework;
using UnityEngine;
using UnityEngine.Pool;

namespace CodeTao
{
    public class UnitPool<T> : ObjectPool<T> where T : UnitController
    {
        public UnitPool(T defaultPrefab, Transform parent = null, int defaultCapacity = 10) :
            base(() =>
                {
                    T instance = parent ? Object.Instantiate(defaultPrefab, parent) : Object.Instantiate(defaultPrefab);
                    return instance;
                }, prefab =>
                {
                    prefab.PreInit();
                    prefab.Parent(parent);
                }
                , prefab =>
                {
                    prefab.gameObject.SetActive(false);
                    prefab.onDeinit = null;
                }
                , prefab =>
                {
                    Object.Destroy(prefab);
                }
                , true, defaultCapacity)
        {
        }
    }

    
}