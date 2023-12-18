using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public partial class Inventory : Container<Item>
    {
        public List<Weapon> weaponInventory = new List<Weapon>();

        private void Start()
        {
            List<Item> items = ComponentUtil.GetComponentsInDescendants<Item>(this);
            foreach (var item in items)
            {
                item.AddToContainer(this);
            }
        }

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
                weaponInventory.Add(item as Weapon);
            }
            else if (item.GetType() == typeof(Artefact) || item.CompareTag("Artefact"))
            {
                parent = Items;
            }
            
            if (item.transform.parent != parent)
            {
                item.transform.SetParent(parent, false);
            }
        }
        
        public bool ContainsItem(Item item)
        {
            return Contents.Contains(item);
        }

        public void AddWAtModGroup(EWAt at, ModifierGroup modGroup)
        {
            foreach (var weapon in weaponInventory)
            {
                weapon.ats[at].AddModifierGroup(modGroup);
            }
        }
    }
}