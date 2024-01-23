using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Pool;

namespace CodeTao
{
    /// <summary>
    /// 单位对象池，不会在获取时enable，但会在回收时disable。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnitPool<T> : ObjectPool<T> where T : UnitController
    {
        public T prefab;
        
        public UnitPool(T defaultPrefab, Transform parent = null, int defaultCapacity = 100) :
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
            prefab = defaultPrefab;
            
        }
    }

    
}