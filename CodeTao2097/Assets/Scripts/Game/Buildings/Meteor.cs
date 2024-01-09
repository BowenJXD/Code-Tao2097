using System;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 流星，建筑物的子类，用于掉落。
    /// </summary>
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