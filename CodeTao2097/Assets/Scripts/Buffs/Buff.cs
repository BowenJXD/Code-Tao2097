using System;
using System.Collections.Generic;
using CodeTao;
using QFramework;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace CodeTao
{
    /// <summary>
    /// Base class for all buffs. Has a loop task to trigger buff and end buff.
    /// </summary>
    public abstract class Buff : Content<Buff>
    {
        public ElementType elementType;
        public List<TriggerCondition> triggerConditions = new List<TriggerCondition>();
        public BindableStat triggerInterval = new BindableStat(0);
        public BindableStat duration = new BindableStat(5);
        public BindableProperty<float> firstTriggerDelay = new BindableProperty<float>(0);
        public LoopTask buffLoop;
        protected BuffOwner buffOwner;
        public ContentPool<Buff> pool;

        public override void OnAdd()
        {
            base.OnAdd();
            Init();
        }

        public virtual void Init()
        {
            buffOwner = (BuffOwner) Container;
            
            float interval = triggerInterval <= 0f? float.MaxValue : triggerInterval;
            buffLoop = new LoopTask(buffOwner, interval, Trigger, Remove);
            
            triggerInterval.RegisterWithInitValue(value =>
                {
                    buffLoop.LoopInterval = value <= 0f? float.MaxValue : value;
                })
                .UnRegisterWhenGameObjectDestroyed(this);
            
            if (duration > 0)
            {
                duration.RegisterWithInitValue(value => { buffLoop.SetTimeCondition(value); })
                    .UnRegisterWhenGameObjectDestroyed(this);
            }

            if (firstTriggerDelay > 0)
            {
                ActionKit.Delay(firstTriggerDelay, () => buffLoop?.Start()).Start(this);
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
        
        /// <summary>
        /// Called inside
        /// </summary>
        void Remove()
        {
            RemoveFromContainer(buffOwner);
        }
        
        /// <summary>
        /// Called outside
        /// </summary>
        public override void OnRemove()
        {
            base.OnRemove();
            buffLoop?.Pause();
            buffLoop = null;
        }
    }
    
}