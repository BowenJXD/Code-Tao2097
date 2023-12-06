using System;
using System.Collections.Generic;
using QFramework;

namespace CodeTao
{
    public partial class Inventory : Container<Item>
    {
        public override void ProcessAddedContent(Content<Item> content)
        {
            Item item = (Item) content;
            item.transform.SetParent(transform);
        }
    }
}