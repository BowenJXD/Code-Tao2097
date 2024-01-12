using System;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class AnimateObject : MonoBehaviour
    {
        protected Animator ani;
        protected AnimationEventListener aniEventListener;
        
        public Action onTrigger;
        public Action onEnd;
        
        public void Awake()
        {
            if (!ani) { ani = GetComponent<Animator>(); }
            if (!aniEventListener) aniEventListener = GetComponent<AnimationEventListener>();
            aniEventListener?.SetListener(OnTrigger);
        }

        private void OnEnable()
        {
            ani.Rebind();
            float aniLength = ani.GetCurrentAnimatorStateInfo(0).length - 0.05f;
            ActionKit.Delay(aniLength, () =>
            {
                if (!aniEventListener) OnTrigger();
                OnEnd();
            }).Start(this);
        }

        void OnTrigger()
        {
            onTrigger?.Invoke();
            onTrigger = null;
        }

        void OnEnd()
        {
            onEnd?.Invoke();
            onEnd = null;
        }
    }
}