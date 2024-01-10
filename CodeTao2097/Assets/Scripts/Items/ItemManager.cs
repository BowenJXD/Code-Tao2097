using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 物品管理器，管理所有物品，提供获取随机物品的方法
    /// </summary>
    public class ItemManager : MonoSingleton<ItemManager>
    {
        public List<Item> items = new List<Item>();
        public List<Weapon> weapons = new List<Weapon>();
        public List<Artefact> artefacts = new List<Artefact>();
        public List<Blessing> blessings = new List<Blessing>();

        private void Start()
        {
            items = FindObjectsOfType<Item>(true).ToList();
            foreach (var item in items)
            {
                if (item is Weapon weapon)
                {
                    weapons.Add(weapon);
                }
                else if (item is Artefact artefact)
                {
                    artefacts.Add(artefact);
                }
                else if (item is Blessing blessing)
                {
                    blessings.Add(blessing);
                }
            }
        }
        
        public List<Item> GetRandomItems(int count)
        {
            if (items == null || items.Count == 0) return null;
            return RandomUtil.GetRandomItems(items, count, GetItemWeight);
        }
        
        int GetItemWeight(Item item)
        {
            int baseWeight = item.GetWeight();
            int elementWeight = ElementSystem.Instance.GetElementWeight(item.relatedElements);
            return baseWeight * elementWeight;
        }
        
        public List<Item> GetRandomUpgradeItems(int count)
        {
            if (items == null || items.Count == 0) return null;
            return RandomUtil.GetRandomItems(items, count, GetUpgradeItemWeight);
        }

        int GetUpgradeItemWeight(Item item)
        {
            if (item.LVL.Value == item.MaxLVL) return 0;
            return GetItemWeight(item);
        }
    }
}