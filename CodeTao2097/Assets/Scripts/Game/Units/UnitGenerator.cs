﻿using System;
using QFramework;

namespace CodeTao
{
    /// <summary>
    /// 单位管理器基类，有单位对象池的单例，供获取单位。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public abstract class UnitGenerator<T, V> : MonoSingleton<V> where V : MonoSingleton<V> where T : UnitController
    {
        protected UnitPool<T> pool;
        public T prefab;
        
        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            if (!prefab)
            {
                prefab = this.GetComponentInDescendants<T>(true);
            }
            pool = new UnitPool<T>(prefab, transform);
        }
        
        public Action<T> onUnitGet;
        
        public virtual T Get()
        {
            T obj = pool.Get();
            onUnitGet?.Invoke(obj);
            obj.onDeinit += () => pool.Release(obj);
            return obj;
        }
    }
}