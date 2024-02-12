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
        public Action<UnitController> onRelease;
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
            obj.onDeinit += () => Release(obj);
            onGet?.Invoke(obj);
            return obj as T;
        }
        
        public void Release(UnitController obj)
        {
            pool.Release(obj);
            onRelease?.Invoke(obj);
        }
    }
    
    public class UnitManager : MonoSingleton<UnitManager>
    {
        public Transform managerPrefab;

        Dictionary<UnitController, PoolRegistry> poolDict = new Dictionary<UnitController, PoolRegistry>();
        
        Dictionary<Type, Action<UnitController>> pendingGetActions = new Dictionary<Type, Action<UnitController>>();
        Dictionary<Type, Action<UnitController>> pendingReleaseActions = new Dictionary<Type, Action<UnitController>>();

        public PoolRegistry Register<T>(T prefab, Transform parent = null, int capacity = 100) where T : UnitController
        {
            if (!poolDict.ContainsKey(prefab))
            {
                if (!parent){
                    parent = Instantiate(managerPrefab, transform);
                }
                poolDict.Add(prefab, new PoolRegistry(prefab, parent, capacity));
                if (pendingGetActions.ContainsKey(typeof(T)))
                {
                    poolDict[prefab].onGet += pendingGetActions[typeof(T)];
                }
                if (pendingReleaseActions.ContainsKey(typeof(T)))
                {
                    poolDict[prefab].onRelease += pendingReleaseActions[typeof(T)];
                }
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

        public List<UnitController> FindAll(Func<UnitController, bool> condition = null)
        {
            List<UnitController> result = new List<UnitController>();
            if (condition == null) condition = _ => true;
            foreach (var kvp in poolDict)
            {
                result.AddRange(kvp.Value.poolList.FindAll(unit => condition(unit)));
            }
            return result;
        }

        public void AddOnGetAction<T>(T prefab, Action<T> action) where T : UnitController
        {
            PoolRegistry registry = poolDict.ContainsKey(prefab) ? poolDict[prefab] : Register(prefab);
            registry.onGet += unit => action(unit as T);
        }
        
        public void AddOnGetAction<T>(Action<T> action) where T : UnitController
        {
            Type type = typeof(T);
            foreach (PoolRegistry registry in poolDict.Values)
            {
                if (registry.type == type)
                {
                    registry.onGet += unit => action(unit as T);
                }
            }

            if (!pendingGetActions.ContainsKey(type))
            {
                pendingGetActions.Add(type, unit => action(unit as T));
            }
        }
        
        public void AddOnGetAction(Type type, Action<UnitController> action)
        {
            foreach (PoolRegistry registry in poolDict.Values)
            {
                if (registry.type == type)
                {
                    registry.onGet += action;
                }
            }

            if (!pendingGetActions.ContainsKey(type))
            {
                pendingGetActions.Add(type, action);
            }
        }
        
        public void AddOnReleaseAction<T>(T prefab, Action<T> action) where T : UnitController
        {
            PoolRegistry registry = poolDict.ContainsKey(prefab) ? poolDict[prefab] : Register(prefab);
            registry.onRelease += unit => action(unit as T);
        }
        
        public void AddOnReleaseAction<T>(Action<T> action) where T : UnitController
        {
            Type type = typeof(T);
            foreach (PoolRegistry registry in poolDict.Values)
            {
                if (registry.type == type)
                {
                    registry.onRelease += unit => action(unit as T);
                }
            }

            if (!pendingReleaseActions.ContainsKey(type))
            {
                pendingReleaseActions.Add(type, unit => action(unit as T));
            }
        }
        
        public void AddOnReleaseAction(Type type, Action<UnitController> action)
        {
            foreach (PoolRegistry registry in poolDict.Values)
            {
                if (registry.type == type)
                {
                    registry.onRelease += action;
                }
            }

            if (!pendingReleaseActions.ContainsKey(type))
            {
                pendingReleaseActions.Add(type, action);
            }
        }
    }
}