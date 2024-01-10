using System;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace CodeTao
{
    /// <summary>
    /// 内容对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ContentPool<T> : ObjectPool<T> where T : Content<T>
    {
        static T defaultPrefab;
        
        public ContentPool(T defaultPrefab, int defaultCapacity = 10) : 
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
                    prefab.AddAfter = null;
                    prefab.RemoveAfter = null;
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