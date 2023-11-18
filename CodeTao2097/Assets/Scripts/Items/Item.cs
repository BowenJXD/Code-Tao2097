using System;
using QFramework;

namespace CodeTao
{
    public class Item : ViewController, IContent<Item>
    {
        public IContainer<Item> Container { get; set; }
        public Action<IContent<Item>> AddAfter { get; set; }
        public Action<IContent<Item>> RemoveAfter { get; set; }
    }
}