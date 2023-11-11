using System;
using System.Collections.Generic;

namespace QFramework
{
    public interface IDelayTimeSystem : ISystem
    {
        /// <summary>
        /// 延时任务
        /// </summary>
        /// <param name="delayTime">延时时间</param>
        /// <param name="onFinished">当计时完成</param>
        DelayTask AddDelayTask(float delayTime, Action onFinished, bool isLoop = false);
    }
    /// <summary>
    /// 延时系统
    /// </summary>
    public class DelayTimeSystem : AbstractSystem, IDelayTimeSystem
    {
        // 可用队列
        private Queue<LinkedListNode<DelayTask>> mAvailableQueue;
        // 所有需要更新的延迟任务
        private LinkedList<DelayTask> mDelayTasks;
        /// <summary>
        /// 添加延时任务
        /// </summary>
        DelayTask IDelayTimeSystem.AddDelayTask(float delayTime, Action onFinished, bool isLoop)
        {
            var node = mAvailableQueue.Count == 0 ?
                new LinkedListNode<DelayTask>(new DelayTask()) : mAvailableQueue.Dequeue();
            node.Value.Start(delayTime, isLoop, onFinished);
            mDelayTasks.AddLast(node);
            return node.Value;
        }
        protected override void OnInit()
        {
            mDelayTasks = new LinkedList<DelayTask>();
            mAvailableQueue = new Queue<LinkedListNode<DelayTask>>();

            PublicMono.Instance.OnUpdate += Update;
        }
        /// <summary>
        /// 更新计时器
        /// </summary>
        private void Update()
        {
            if (mDelayTasks.Count == 0) return;
            // 拿到第一个任务
            var currentNode = mDelayTasks.First;
            // 如果当前有任务需要更新
            while (currentNode != null)
            {
                // 缓存下一个节点
                var nextNode = currentNode.Next;
                // 获取当前任务
                var task = currentNode.Value;
                // 更新任务
                task.Update();
                // 当任务完成 移除任务
                if (task.IsFinish)
                {
                    mAvailableQueue.Enqueue(currentNode);
                    mDelayTasks.Remove(currentNode);
                }
                // 将下一个节点 赋值给 当前节点
                currentNode = nextNode;
            }
        }
    }
}