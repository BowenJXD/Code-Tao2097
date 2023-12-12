using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public partial class Inventory : Container<Item>
    {
        public override void ProcessAddedContent(Content<Item> content)
        {
            Item item = content as Item;
            if (item == null)
            {
                return;
            }
            
            Transform parent = transform;
            
            if (item.GetType() == typeof(Weapon) || item.CompareTag("Weapon"))
            {
                parent = Weapons;
            }
            else if (item.GetType() == typeof(Item) || item.CompareTag("Item"))
            {
                parent = Items;
            }
            
            if (item.transform.parent != parent)
            {
                item.transform.SetParent(parent, false);
            }
        }
    }
}