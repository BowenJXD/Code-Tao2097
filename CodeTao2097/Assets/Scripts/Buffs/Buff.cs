using System;
using System.Collections.Generic;
using CodeTao;
using QFramework;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace CodeTao
{
    public class Buff : Content<Buff>
    {
        public List<TriggerCondition> triggerConditions = new List<TriggerCondition>();
        public BindableStat triggerInterval = new BindableStat(0);
        public BindableStat duration = new BindableStat(5);
        public BindableProperty<float> firstTriggerDelay = new BindableProperty<float>(0);
        public LoopTask buffLoop;
        protected BuffOwner buffOwner;

        public override void OnAdd()
        {
            base.OnAdd();
            Init();
        }

        public virtual void Init()
        {
            buffOwner = (BuffOwner) Container;
            
            if (triggerInterval <= 0f) triggerInterval.Value = float.MaxValue;
            
            buffLoop = new LoopTask(buffOwner, triggerInterval, Trigger, Remove);
            
            triggerInterval.RegisterWithInitValue(value => { buffLoop.LoopInterval = value; })
                .UnRegisterWhenGameObjectDestroyed(this);
            
            if (duration > 0)
            {
                duration.RegisterWithInitValue(value => { buffLoop.SetTimeCondition(value); })
                    .UnRegisterWhenGameObjectDestroyed(this);
            }

            if (firstTriggerDelay > 0)
            {
                ActionKit.Delay(firstTriggerDelay, () => buffLoop.Start()).Start(this);
            }
            else
            {
                buffLoop.Start();
            }
            
            foreach (TriggerCondition triggerCondition in triggerConditions)
            {
                triggerCondition.buffToTrigger = this;
                triggerCondition.Init();
            }
        }
        
        public virtual void Trigger(){}
        
        public virtual void Remove()
        {
            buffLoop = null;
            RemoveFromContainer(Container);
        }
        
        public override void OnRemove()
        {
            base.OnRemove();
            buffLoop?.Pause();
            buffLoop = null;
        }
    }
    
}