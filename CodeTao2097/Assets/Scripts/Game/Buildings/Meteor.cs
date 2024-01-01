using System;
using UnityEngine;

namespace CodeTao
{
    public class Meteor : Building
    {
        protected Animator ani;
        protected AnimationEventListener aniEventListener;

        public Action onFall;
        
        public override void PreInit()
        {
            base.PreInit();
            if (!ani)
            {
                ani = this.GetComponentInDescendants<Animator>();
            }
            else
            {
                ani.enabled = true;
                ani.Rebind();
            }
            aniEventListener = ani.GetComponent<AnimationEventListener>();
            onFall += OnFall;
        }
        
        public override void Init()
        {
            base.Init();
            col2D.enabled = false;
            aniEventListener.SetListener(onFall);
        }

        void OnFall()
        {
            col2D.enabled = true;
            ani.enabled = false;
            onFall = null;
        }
    }
}