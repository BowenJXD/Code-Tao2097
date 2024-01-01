using System;
using UnityEngine;

namespace CodeTao
{
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