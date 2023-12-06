using System;
using QFramework;

namespace CodeTao
{
    public class Blessing : Content<Blessing>
    {
        public BindableProperty<int> weight = new BindableProperty<int>(1);

        public int GetWeight()
        {
            return weight.Value;
        }
    }
}