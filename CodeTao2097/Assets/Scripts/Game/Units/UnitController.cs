using System;
using QFramework;

namespace CodeTao
{
    /// <summary>
    /// 单位控制器，用于管理单位的生命周期
    /// </summary>
    public abstract class UnitController : ViewController
    {
        /// <summary>
        /// Will be called from global when scene is loaded,
        /// Used to bind components
        /// </summary>
        public virtual void OnSceneLoaded(){}
        
        /// <summary>
        /// Called from UnitPool
        /// </summary>
        public virtual void PreInit(){}
        
        public Action onInit;
        
        /// <summary>
        /// Set gameObject to be active, need to be called manually
        /// </summary>
        public virtual void Init()
        {
            onInit?.Invoke();
            onInit = null;
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Used by UnitPool to return gameObject to pool
        /// </summary>
        public Action onDeinit;
        
        /// <summary>
        /// Used to deactivate gameObject
        /// </summary>
        public virtual void Deinit()
        {
            onDeinit?.Invoke();
            onDeinit = null;
        }
    }
}