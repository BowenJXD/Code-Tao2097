using System;
using QFramework;
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
        
        public override void SetUp()
        {
            base.SetUp();
            if (!ani) ani = this.GetComponentInDescendants<Animator>();
            aniEventListener = ani.GetComponent<AnimationEventListener>();
        }
        
        public override void Init()
        {
            base.Init();
            ani.enabled = true;
            ani.Rebind();
            col2D.enabled = false;
            aniEventListener.SetListener(onFall);
            onFall += OnFall;
        }

        void OnFall()
        {
            col2D.enabled = true;
            ani.enabled = false;
            onFall = null;
        }
    }
}