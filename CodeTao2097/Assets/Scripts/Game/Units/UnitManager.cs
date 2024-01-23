using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;
using UnityEngine.Pool;

namespace CodeTao
{
    public class PoolRegistry
    {
        public Type type { get; private set; }
        public Action<UnitController> onGet;
        public UnitPool<UnitController> pool;
        public List<UnitController> poolList = new List<UnitController>();
        
        public PoolRegistry(UnitController prefab, Transform parent = null, int capacity = 100)
        {
            this.type = prefab.GetType();
            pool = new UnitPool<UnitController>(prefab, parent, capacity);
        }
        
        public T Get<T>() where T : UnitController
        {
            UnitController obj = pool.Get();
            if (poolList.Count < pool.CountAll) poolList.Add(obj);
            obj.onDeinit += () => pool.Release(obj);
            onGet?.Invoke(obj);
            return obj as T;
        }
    }
    
    public class UnitManager : MonoSingleton<UnitManager>
    {
        public Transform managerPrefab;

        public Dictionary<UnitController, PoolRegistry> poolDict = new Dictionary<UnitController, PoolRegistry>(); 

        public PoolRegistry Register<T>(T prefab, Transform parent = null, int capacity = 100) where T : UnitController
        {
            if (!poolDict.ContainsKey(prefab))
            {
                if (!parent){
                    parent = Instantiate(managerPrefab, transform);
                }
                poolDict.Add(prefab, new PoolRegistry(prefab, parent, capacity));
            }
            return poolDict[prefab];
        }
        
        public T Get<T>(T prefab) where T : UnitController
        {
            poolDict.TryGetValue(prefab, out PoolRegistry registry);
            if (registry == null)
            {
                registry = Register<T>(prefab);
            }
            return registry.Get<T>();
        }
        
        public T[] GetAll<T>(T prefab) where T : UnitController
        {
            poolDict.TryGetValue(prefab, out PoolRegistry registry);
            if (registry == null)
            {
                registry = Register<T>(prefab);
            }
            return registry.poolList.ToArray() as T[];
        }
        
        public T[] GetAll<T>() where T : UnitController
        {
            return poolDict.Values.ToList().FindAll(registry => registry.type == typeof(T)).SelectMany(registry => registry.poolList).ToArray() as T[];
        }
        
        public void AddOnGetAction<T>(T prefab, Action<T> action) where T : UnitController
        {
            poolDict.TryGetValue(prefab, out PoolRegistry registry);
            if (registry == null)
            {
                registry = Register<T>(prefab);
            }
            registry.onGet += unit => action(unit as T);
        }
        
        public void AddOnGetAction<T>(Action<T> action) where T : UnitController
        {
            foreach (PoolRegistry registry in poolDict.Values)
            {
                if (registry.type == typeof(T))
                {
                    registry.onGet += unit => action(unit as T);
                }
            }
        }
    }
}