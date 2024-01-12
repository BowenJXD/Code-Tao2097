using System;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 用于跨game object绑定动画事件的组件
    /// </summary>
    public class AnimationEventListener : MonoBehaviour
    {
        public Action onAniEvent;
        
        public void SetListener(Action action)
        {
            onAniEvent = action;
        }
        
        public void AnimationEvent()
        {
            onAniEvent?.Invoke();
        }
    }
}