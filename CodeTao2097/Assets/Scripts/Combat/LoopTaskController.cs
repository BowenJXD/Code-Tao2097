using System;
using QFramework;
using UnityEngine;
using UnityEngine.Events;

namespace CodeTao
{
    public class LoopTaskController : UnitComponent, IWAtReceiver
    {
        public LoopTask loopTask;
        public BindableStat interval = new BindableStat();
        public BindableStat duration = new BindableStat();
        public Action trigger;
        public Action finish;

        void OnEnable()
        {
            interval.Init();
            loopTask = new LoopTask(this, float.MaxValue, OnTrigger, OnFinish);
            interval.RegisterWithInitValue(value =>
            {
                if (value != 0) loopTask.LoopInterval = value;
            }).UnRegisterWhenGameObjectDestroyed(this);
            duration.Init();
            duration.RegisterWithInitValue(value =>
            {
                if (value != 0) loopTask.SetTimeCondition(value);
            }).UnRegisterWhenGameObjectDestroyed(this);
            loopTask.Start();
        }

        public void AddTrigger(Action action)
        {
            trigger += action;
        }
        
        void OnTrigger()
        {
            trigger?.Invoke();
        }
        
        public void AddFinish(Action action)
        {
            finish += action;
        }

        void OnFinish()
        {
            finish?.Invoke();
        }

        public void Receive(IWAtSource source)
        {
            interval.InheritStat(source.GetWAt(EWAt.Cooldown));
            duration.InheritStat(source.GetWAt(EWAt.Duration));
        }

        private void OnDisable()
        {
            interval.Reset();
            duration.Reset();
            loopTask = null;
            trigger = null;
            finish = null;
        }
    }
}