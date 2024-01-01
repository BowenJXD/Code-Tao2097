using System;
using QFramework;

namespace CodeTao
{
    public abstract class UnitController : ViewController
    {
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