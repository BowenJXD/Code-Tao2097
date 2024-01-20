﻿using System;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 内容，一对多关系中的多，可以升级，可以被添加到容器中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class Content<T> : MonoBehaviour where T : Content<T>
    {
        #region Add&Remove
        
        [HideInInspector] public Container<T> Container;

        public Action<T> AddAfter;

        public Action<T> RemoveAfter;
        
        [TabGroup("Content")]
        public RepetitionBehavior repetitionBehavior = RepetitionBehavior.Return;

        public virtual bool AddToContainer(Container<T> container)
        {
            bool result = container.AddContent(this, repetitionBehavior);
            if (result)
            {
                Container = container;
                OnAdd();
                AddAfter?.Invoke((T)this);
            }

            return result;
        }

        public virtual void OnAdd()
        {
            if (LVL == 0)
            {
                AlterLVL();
            }
        }

        public virtual bool RemoveFromContainer(Container<T> container)
        {
            bool result = container.RemoveContent(this);
            if (result)
            {
                Container = null;
                OnRemove();
                RemoveAfter?.Invoke((T)this);
            }

            return result;
        }
        
        public virtual void OnRemove(){}
        
        #endregion

        #region Level

        [TabGroup("Content")]
        public BindableProperty<int> LVL = new BindableProperty<int>(0);
        
        [TabGroup("Content")]
        public BindableProperty<int> MaxLVL = new BindableProperty<int>(9);

        public virtual bool Stack(Content<T> newContent)
        {
            return SetLVL(LVL.Value + newContent.LVL.Value);
        }
        
        public virtual bool SetLVL(int lvl)
        {
            if (lvl > MaxLVL.Value)
            {
                lvl = MaxLVL.Value;
            }
            else if (lvl <= 0)
            {
                RemoveFromContainer(Container);
            }
            if (lvl == LVL.Value)
            {
                return false;
            }
            
            LVL.Value = lvl;
            return true;
        }

        public virtual void AlterLVL(int lvlIncrement = 1)
        {
            SetLVL(LVL.Value + lvlIncrement);
        }
        
        #endregion
        
        public override bool Equals(object other)
        {
            if (other == null || gameObject == null)
            {
                return false;
            }
            T content = other as T;
            if (content)
            {
                return gameObject.name == content.gameObject.name;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}