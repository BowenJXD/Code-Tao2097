using System;
using QFramework;

namespace CodeTao
{
    public class Blessing : Content<Blessing>
    {
        public BindableProperty<int> weight = new BindableProperty<int>(1);

        public virtual int GetWeight()
        {
            return weight.Value;
        }

        public override void OnRemove()
        {
            base.OnRemove();
            enabled = false;
        }
    }
}