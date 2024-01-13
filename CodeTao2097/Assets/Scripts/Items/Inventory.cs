using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 物品栏组件，用于记录玩家拥有的物品
    /// </summary>
    public partial class Inventory : Container<Item>
    {
        public List<Weapon> weaponInventory = new List<Weapon>();
        public List<Artefact> artefactInventory = new List<Artefact>();
        public List<Blessing> blessingInventory = new List<Blessing>();
        
        private void Start()
        {
            List<Item> items = this.GetComponentsInDescendants<Item>();
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
                parent = Artefacts;
                artefactInventory.Add(item as Artefact);
            }
            else if (item.GetType() == typeof(Blessing) || item.CompareTag("Blessing"))
            {
                parent = Blessings;
                blessingInventory.Add(item as Blessing);
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
                weapon.GetWAtStat(at).AddModifierGroup(modGroup);
            }
        }
    }
}