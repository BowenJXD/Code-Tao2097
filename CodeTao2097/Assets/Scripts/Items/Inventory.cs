using System.Collections.Generic;
using QFramework;

namespace CodeTao
{
    public class Inventory : ViewController, IContainer<Item>
    {
        public List<IContent<Item>> Contents { get; set; }
    }
}