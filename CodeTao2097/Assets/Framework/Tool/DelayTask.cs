using System;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 延时任务
    /// </summary>
    public class DelayTask
    {
        /// <summary>
        /// 任务状态枚举
        /// </summary>
        public enum E_State { Start, Pause, Finished }
        // 当计时结束做的事情
        private Action mTask;
        // 剩余时间
        private float mTimeRemaining;
        // 延迟时间
        private float mDelayTime;
        // 是否循环
        private bool mLoop;
        // 任务状态
        private E_State mState = E_State.Finished;
        /// <summary>
        /// 任务是否完成
        /// </summary>
        public bool IsFinish => mState == E_State.Finished;
        /// <summary>
        /// 剩余时间
        /// </summary>
        public float TimeRemaining => mTimeRemaining;
        /// <summary>
        /// 开始计时
        /// </summary>
        public void Start(float delayTime, bool isLoop, Action task = null)
        {
            mTask = task;
            mDelayTime = delayTime;
            mLoop = isLoop;
            // 重置计时器
            mTimeRemaining = mDelayTime;
            // 开启任务
            mState = E_State.Start;
        }
        /// <summary>
        /// 暂停任务
        /// </summary>
        public void Pause()
        {
            mState = E_State.Pause;
        }
        /// <summary>
        /// 继续任务
        /// </summary>
        public void Continue()
        {
            // 如果在不是暂停状态 就不让他继续
            if (mState != E_State.Pause) return;
            // 如果是循环任务 就重置剩余时间重新开始
            if (mLoop) mTimeRemaining = mDelayTime;
            // 将任务设置为开始
            mState = E_State.Start;
        }
        /// <summary>
        /// 提供给外部手动停止的方法
        /// </summary>
        public void Stop()
        {
            // 将任务设置为完成
            mState = E_State.Finished;
            // 将当前任务取消
            mTask = null;
        }
        /// <summary>
        /// 更新计时器
        /// </summary>
        public void Update()
        {
            // 如果不是开始状态就不执行计时
            if (mState != E_State.Start) return;
            // 如果剩余时间大于 0 说明计时未结束
            if (mTimeRemaining > 0)
            {
                mTimeRemaining -= Time.deltaTime;
                return;
            }
            // 如果剩余时间小于等于 0 就执行任务
            mTask?.Invoke();
            // 到达完成时间 如果不是循环 就停止计时器
            if (!mLoop) Stop();
            // 否则重置剩余时间
            else mTimeRemaining = mDelayTime;
        }
    }
}