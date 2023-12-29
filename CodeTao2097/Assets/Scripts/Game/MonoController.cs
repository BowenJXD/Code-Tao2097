using System;
using QFramework;

namespace CodeTao
{
    public class MonoController : ViewController
    {
        public Action onEnable;
        private void OnEnable()
        {
            onEnable?.Invoke();
            onEnable = null;
        }

        public Action onDisable;
        private void OnDisable()
        {
            onDisable?.Invoke();
            onDisable = null;
        }
        
    }
}