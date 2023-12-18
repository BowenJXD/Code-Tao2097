using System;
using QFramework;

namespace CodeTao
{
    [Serializable]
    public class TriggerCondition
    {
        public BindableProperty<float> triggerCooldown = new BindableProperty<float>(0);
        
        public Buff buffToTrigger;
        
        public virtual void Init()
        {
            
        }
    }
}