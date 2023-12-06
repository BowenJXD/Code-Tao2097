using System;
using QFramework;

namespace CodeTao
{
    public abstract class Item : Content<Item>
    {
        protected virtual void Start()
        {
            LVL = new BindableProperty<int>(1);
            MaxLVL = new BindableProperty<int>(10);
        }

        public BindableStat weight = new BindableStat(10);
        
        public virtual int GetWeight()
        {
            return (int)weight.Value;
        }

        public virtual string GetDescription()
        {
            return $"{GetType()} ++";
        }
    }
}