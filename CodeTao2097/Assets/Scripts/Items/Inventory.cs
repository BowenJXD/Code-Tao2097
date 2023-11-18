using System;
using System.Collections.Generic;
using QFramework;

namespace CodeTao
{
    public class Inventory : ViewController, IContainer<Item>
    {
        public List<IContent<Item>> Contents { get; set; }
        public Action<IContent<Item>> AddAfter { get; set; }
        public Action<IContent<Item>> RemoveAfter { get; set; }

        public void ProcessAddedContent(IContent<Item> content)
        {
            Item item = (Item) content;
            item.transform.SetParent(transform);
        }
    }
}