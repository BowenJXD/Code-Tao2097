using System;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeTao
{
    [Serializable]
    public abstract class Content<T> : ViewController where T : Content<T>
    {
        #region Add&Remove
        
        [HideInInspector] public Container<T> Container;

        public Action<Content<T>> AddAfter;

        public Action<Content<T>> RemoveAfter;

        public virtual bool AddToContainer(Container<T> container, ERepetitionBehavior repetitionBehavior = ERepetitionBehavior.Return)
        {
            bool result = container.AddContent(this, repetitionBehavior);
            if (result)
            {
                Container = container;
                OnAdd();
                AddAfter?.Invoke(this);
            }

            return result;
        }

        public virtual void OnAdd(){}

        public virtual bool RemoveFromContainer(Container<T> container)
        {
            bool result = container.RemoveContent(this);
            if (result)
            {
                Container = null;
                OnRemove();
                RemoveAfter?.Invoke(this);
            }

            return result;
        }
        
        public virtual void OnRemove(){}
        
        #endregion
        
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
            else if (lvl < 1)
            {
                lvl = 1;
            }
            if (lvl == LVL.Value)
            {
                return false;
            }
            
            LVL.Value = lvl;
            return true;
        }

        public virtual void Upgrade(int lvlIncrement = 1)
        {
            LVL.Value += lvlIncrement;
        }
        
        public override bool Equals(object other)
        {
            return GetType() == other.GetType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}