using System;
using QFramework;

namespace CodeTao
{
    public class Blessing : IContent<Blessing>
    {
        #region IContent
        public IContainer<Blessing> Container { get; set; }
        public Action<IContent<Blessing>> AddAfter { get; set; }
        public Action<IContent<Blessing>> RemoveAfter { get; set; }
        public BindableProperty<int> LVL { get; set; }
        public BindableProperty<int> MaxLVL { get; set; }
        #endregion

        public BindableProperty<int> weight = new BindableProperty<int>(1);

        public int GetWeight()
        {
            return weight.Value;
        }
    }
}