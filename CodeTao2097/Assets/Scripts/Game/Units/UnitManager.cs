using QFramework;

namespace CodeTao
{
    public abstract class UnitManager<T> : MonoSingleton<UnitManager<T>> where T : UnitController
    {
        protected UnitPool<T> pool;
        public T prefab;
        
        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            if (!prefab)
            {
                prefab = this.GetComponentInDescendants<T>();
            }
            pool = new UnitPool<T>(prefab, transform);
        }
        
        public virtual T Get()
        {
            T obj = pool.Get();
            obj.onDeinit += () => pool.Release(obj);
            return obj;
        }
    }
}