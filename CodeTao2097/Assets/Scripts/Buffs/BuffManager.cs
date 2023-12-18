using QFramework;
using UnityEngine;
using UnityEngine.Pool;

namespace CodeTao
{
    public class BuffManager : MonoSingleton<BuffManager>
    {
        
    }
    
    public class BuffPool<T> : ObjectPool<T> where T : Buff
    {
        public BuffPool(T defaultPrefab, int defaultCapacity = 10) :
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
                    prefab.Parent(BuffManager.Instance);
                    prefab.gameObject.SetActive(false);
                }
                , prefab => { Object.Destroy(prefab); }
                , true, defaultCapacity)
        {
        }
    }
}