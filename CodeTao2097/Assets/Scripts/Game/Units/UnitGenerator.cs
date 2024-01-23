using System;
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
        public T prefab;
        public float spawnCD = 0;
        private bool isInCD;
        
        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            if (!prefab)
            {
                prefab = this.GetComponentInDescendants<T>(true);
            }
            UnitManager.Instance.Register(prefab, transform);
        }
        
        public Action<T> onUnitGet;
        
        public virtual T Get()
        {
            if (isInCD) return null;
            
            T obj = UnitManager.Instance.Get(prefab);
            onUnitGet?.Invoke(obj);

            if (spawnCD > 0)
            {
                isInCD = true;
                ActionKit.Delay(spawnCD, () => isInCD = false).Start(this);
            }
            
            return obj;
        }
    }
}