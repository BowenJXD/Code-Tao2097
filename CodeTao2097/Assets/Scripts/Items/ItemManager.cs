using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class ItemManager : MonoSingleton<ItemManager>
    {
        public List<Item> items = new List<Item>();

        private void Start()
        {
            items = FindObjectsOfType<Item>().ToList();
        }
        
        public List<Item> GetRandomItems(int count)
        {
            if (items == null || items.Count == 0) return null;
            return RandomUtil.GetRandomItems(items, count, item => item.GetWeight());
        }
        
    }
}