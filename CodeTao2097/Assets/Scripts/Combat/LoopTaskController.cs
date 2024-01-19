using System;
using QFramework;
using UnityEngine;
using UnityEngine.Events;

namespace CodeTao
{
    public class LoopTaskController : UnitComponent, IWAtReceiver
    {
        public LoopTask loopTask;
        public BindableStat interval = new BindableStat(1);
        public BindableStat duration = new BindableStat(5);
        public Action trigger;
        public Action finish;

        public void StartTask()
        {
            loopTask = new LoopTask(this, interval.Value, OnTrigger, OnFinish);
            interval.Init();
            interval.RegisterWithInitValue(value =>
            {
                loopTask.LoopInterval = value;
            }).UnRegisterWhenGameObjectDestroyed(this);
            duration.Init();
            duration.RegisterWithInitValue(value =>
            {
                loopTask.SetTimeCondition(duration);
            }).UnRegisterWhenGameObjectDestroyed(this);
            loopTask.Start();
        }

        void OnTrigger()
        {
            if (trigger != null) trigger();
            else SendMessage("Trigger");
        }

        void OnFinish()
        {
            if (finish != null) finish();
            else SendMessage("Finish");
        }

        public void Receive(IWAtSource source)
        {
            interval.InheritStat(source.GetWAt(EWAt.Cooldown));
            duration.InheritStat(source.GetWAt(EWAt.Duration));
        }

        private void OnDisable()
        {
            loopTask = null;
            interval.Reset();
            duration.Reset();
            trigger = null;
            finish = null;
        }
    }
}