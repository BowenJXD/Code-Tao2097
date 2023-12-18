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
        /// Set gameObject to be active
        /// </summary>
        public virtual void Init()
        {
            onInit?.Invoke();
            gameObject.SetActive(true);
        }
        
        public Action onDeinit;
        
        public virtual void Deinit()
        {
            onDeinit?.Invoke();
            onDeinit = null;
        }
    }
}