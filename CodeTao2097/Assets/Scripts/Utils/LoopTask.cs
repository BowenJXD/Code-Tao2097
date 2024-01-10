using System;
using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using QFramework;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 以一定间隔重复执行单个效果的任务
    /// </summary>
    public class LoopTask
    {

        private Action _loopTask;

        private Action _endTask;

        private MonoBehaviour _owner;
        
        [SerializeField][ReadOnly] private float _loopTime;
        
        [SerializeField][ReadOnly] private int _loopCount;

        [SerializeField][ReadOnly] private float _loopInterval;

        [SerializeField][ReadOnly] bool _isPaused;

        public float LoopInterval
        {
            get => _loopInterval;
            set { _loopInterval = value; }
        }

        #region condition

        private Func<bool> _condition;

        public bool CheckCondition => _condition?.Invoke() ?? true;

        /// <summary>
        /// input true for loop forever
        /// input false for loop once
        /// </summary>
        public void SetBoolCondition(bool boolCondition)
        {
            _condition = () => { return boolCondition; };
        }

        /// <summary>
        /// Set how much time the loop will run.
        /// </summary>
        public void SetTimeCondition(float timeCondition)
        {
            _condition = () => { return _loopCount * _loopInterval + _loopTime <= timeCondition; };
        }

        /// <summary>
        /// Set how many times the loop will run.
        /// </summary>
        public void SetCountCondition(int countCondition)
        {
            _condition = () => { return _loopCount < countCondition; };
        }

        public void SetCondition(Func<bool> condition)
        {
            _condition = condition;
        }

        #endregion

        /// <summary>
        /// Start the coroutine, the coroutine will run forever in default.
        /// </summary>
        /// <param name="executer">The executer of the coroutine</param>
        /// <param name="loopInterval">The interval between every loop</param>
        /// <param name="loopTask">The task to trigger every time a loop is finished</param>
        /// <param name="endTask">The task to trigger when the coroutine ends</param>
        public LoopTask(MonoBehaviour owner, float loopInterval, Action loopTask = null, Action endTask = null)
        {
            _loopInterval = loopInterval;
            _loopTask = loopTask;
            _endTask = endTask;
            _owner = owner;
        }

        public void Start()
        {
            _condition = null;
            _loopCount = 0;
            _loopTime = 0;
            Resume();
        }
        
        public void Pause()
        {
            _isPaused = true;
            _owner.StopCoroutine(Update());
        }
        
        public void Resume(bool immediateLoop = false)
        {
            if (immediateLoop)
            {
                _loopTime = _loopInterval;
            }
            _isPaused = false;
            _owner.StartCoroutine(Update());
        }

        public void Finish()
        {
            _endTask?.Invoke();
            _loopTask = null;
            _loopCount = 0;
            Pause();
        }

        private IEnumerator Update()
        {
            while (CheckCondition)
            {
                while (_loopTime < _loopInterval)
                {
                    _loopTime += _isPaused? 0 : Time.deltaTime;
                    
#if !UNITY_EDITOR
                    yield return new WaitForSeconds(Time.deltaTime);
#else
                    yield return null;
#endif
                }
                Trigger();
            }
            Finish();
        }

        private void Trigger()
        {
            _loopCount++;
            _loopTime -= _loopInterval;
            _loopTask?.Invoke();
        }
    }
}
