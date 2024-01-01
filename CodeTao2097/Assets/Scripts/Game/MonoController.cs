using System;
using QFramework;

namespace CodeTao
{
    public class MonoController : ViewController
    {
        public Action onEnable;
        protected virtual void OnEnable()
        {
            onEnable?.Invoke();
            onEnable = null;
        }

        public Action onDisable;
        protected virtual void OnDisable()
        {
            onDisable?.Invoke();
            onDisable = null;
        }
        
    }
}