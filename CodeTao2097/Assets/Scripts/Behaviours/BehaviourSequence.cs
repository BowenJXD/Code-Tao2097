using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 行为序列，用于执行一系列行为节点。每当满足时间条件和索引条件时，会执行一次序列。
    /// 包括一个黑板，用于存储行为节点之间的数据交互。
    /// 基于QFramework的ActionKit实现。
    /// </summary>
    public class BehaviourSequence : MonoBehaviour, IWAtReceiver
    {
        /// <summary>
        ///  循环间隔条件。0: 无时间条件。
        /// </summary>
        [Tooltip("The time condition to start a loop of the sequence. \n" +
                 "0: No time condition. ")]
        public BindableStat loopInterval = new(-1);
        protected float loopIntervalTimer = 0;
        protected bool intervalCondMet = false;
        
        /// <summary>
        ///  循环索引条件。0: 无索引条件。-1: 只在最后一个序列结束后循环。n: 在第n个序列结束后循环（假设起始状态为1）。
        /// </summary>
        [Tooltip("The index condition to start a loop of the sequence. \n" +
                 "0: No index condition. \n" +
                 "-1: Only loop after the last sequence is finished. \n" +
                 "n: Loop after the n-th sequence is finished (assume starting state is 1).")]
        public int loopIndexCondition = 0;
        protected bool indexCondMet = false;
        
        protected List<BehaviourNode> nodes;

        [SerializeField] [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
        public SerializableDictionary<string, object> blackboard = new();
        
        protected List<ISequence> sequences = new();
        
        public void Init()
        {
            nodes = this.GetComponents<BehaviourNode>().ToList();
            nodes.AddRange(this.GetComponentsInDescendants<BehaviourNode>());
            loopInterval.Init();
            foreach (var node in nodes)
            {
                node.Init(this);
            }
            intervalCondMet = loopInterval == 0;
            indexCondMet = loopIndexCondition == 0;
            
            TryStartSequence();
        }

        public void Update()
        {
            if (loopInterval > 0)
            {
                loopIntervalTimer += Time.deltaTime;
                if (loopIntervalTimer >= loopInterval)
                {
                    intervalCondMet = true;
                }
            }
            if (loopIndexCondition == -1 && sequences.Count == 0)
            {
                indexCondMet = true;
            }
            TryStartSequence();
        }

        public void TryStartSequence()
        {
            if (intervalCondMet && indexCondMet)
            {
                StartSequence();
                if (loopInterval != 0)
                {
                    intervalCondMet = false;
                    loopIntervalTimer = 0;
                }
                if (loopIndexCondition != 0)
                {
                    indexCondMet = false;
                }
            }
        }
        
        public void StartSequence()
        {
            ISequence sequence = ActionKit.Sequence();
            for (int i = 0; i < nodes.Count; i++)
            {
                sequence.Coroutine(nodes[i].Execute);
                if (loopIndexCondition == i + 1)
                {
                    sequence.Callback(() =>
                    {
                        indexCondMet = true;
                    });
                }
            }
            
            sequences.Add(sequence);
            sequence.Callback(() =>
            {
                sequences.Remove(sequence);
            });
            sequence.Start(this);
        }
        
        public void Set(string key, object value)
        {
            blackboard[key] = value;
        }
        
        public T Get<T>(string key)
        {
            if (!blackboard.ContainsKey(key))
            {
                return default;
            }
            return (T) blackboard[key];
        }

        public void Receive(IWAtSource source)
        {
            loopInterval.InheritStat(source.GetWAt(EWAt.Cooldown));
        }
    }
}