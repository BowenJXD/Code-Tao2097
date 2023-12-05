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
    [Serializable]
    public class GeneratorTask
    {
        public List<int> amountsToGenerate = new List<int>();

        public float duration = 60;

        private float Interval => duration / amountsToGenerate.Sum();
        
        [HideInInspector] public UnityEvent<int> onGenerate = new UnityEvent<int>();
        [HideInInspector] public UnityEvent onFinish = new UnityEvent();

        public LoopTask LoopTask;

        public void Start(MonoBehaviour owner)
        {
            LoopTask = new LoopTask(owner, Interval, Generate, Finish);
            LoopTask.SetTimeCondition(duration);
            LoopTask.Start();
        }

        public void Generate()
        {
            if (amountsToGenerate.Sum() == 0)
            {
                Finish();
                return;
            }
            int index = Util.GetRandomWeightedIndex(amountsToGenerate);
            amountsToGenerate[index] = Mathf.Max(0, amountsToGenerate[index] - 1);
            onGenerate.Invoke(index);
        }

        public void Finish()
        {
            onFinish.Invoke();
        }
    }
}