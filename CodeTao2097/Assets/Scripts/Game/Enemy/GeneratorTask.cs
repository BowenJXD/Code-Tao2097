using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeTao;
using QFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 生成器任务，在给定的时间内生成指定数量的单位。
    /// </summary>
    [Serializable]
    public class GeneratorTask
    {
        public int amountToGenerate = 0;

        public float duration = 60;

        [HideInInspector] public UnityEvent<int> onGenerate = new UnityEvent<int>();
        [HideInInspector] public UnityEvent onFinish = new UnityEvent();

        public LoopTask LoopTask;

        public void Start(MonoBehaviour owner)
        {
            float interval = amountToGenerate > 0? duration / amountToGenerate : float.MaxValue;
            LoopTask = new LoopTask(owner, interval, Generate, Finish);
            LoopTask.SetTimeCondition(duration);
            LoopTask.Start();
        }

        public void Generate()
        {
            if (amountToGenerate == 0)
            {
                Finish();
                return;
            }

            int index = RandomUtil.Rand(amountToGenerate);
            amountToGenerate -= 1;
            onGenerate.Invoke(index);
        }

        public void Finish()
        {
            onFinish.Invoke();
        }
    }
}