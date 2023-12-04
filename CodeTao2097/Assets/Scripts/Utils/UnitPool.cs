using UnityEngine;
using UnityEngine.Pool;

namespace CodeTao
{
    public class UnitPool<T> : ObjectPool<T> where T : UnitController
    {
        public UnitPool(T defaultPrefab, int defaultCapacity = 10) :
            base(() =>
                {
                    T instance = Object.Instantiate(defaultPrefab);
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

    
}