using System;
using QFramework;

namespace CodeTao
{
    public abstract class UnitManager<T, V> : MonoSingleton<V> where V : MonoSingleton<V> where T : UnitController
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