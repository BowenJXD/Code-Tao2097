using System;
using QFramework;

namespace CodeTao
{
    public class UnitController : ViewController
    {
        public Action onInit;
        
        public virtual void Init()
        {
            onInit?.Invoke();
        }
        
        public Action onDestroy;
        
        public virtual void Deinit()
        {
            onDestroy?.Invoke();
            onDestroy = null;
        }
    }
}