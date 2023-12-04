using System;
using QFramework;

namespace CodeTao
{
    public class Item : ViewController, IContent<Item>
    {
        #region IContent
        public IContainer<Item> Container { get; set; }
        public Action<IContent<Item>> AddAfter { get; set; }
        public Action<IContent<Item>> RemoveAfter { get; set; }
        public BindableProperty<int> LVL { get; set; }
        public BindableProperty<int> MaxLVL { get; set; }
        #endregion

    }
}