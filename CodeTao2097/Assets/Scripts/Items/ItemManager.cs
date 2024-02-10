using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using Sirenix.OdinInspector;
using UnityEditor.Localization.Plugins.XLIFF.V12;
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
            GetLists();
        }

        void GetLists()
        {
            items.Clear();
            weapons.Clear();
            artefacts.Clear();
            blessings.Clear();
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
        
        public List<Item> GetRandomUpgradeItems(int count, ItemType[] itemTypes = null)
        {
            if (items == null || items.Count == 0) return null;
            if (itemTypes == null || itemTypes.Length == 0)
            {
                itemTypes = new ItemType[] {};
            }
            List<Item> upgradeItems = new List<Item>();
            foreach (var itemType in itemTypes)
            {
                switch (itemType)
                {
                    case ItemType.Weapon:
                        upgradeItems.AddRange(weapons);
                        break;
                    case ItemType.Artefact:
                        upgradeItems.AddRange(artefacts);
                        break;
                    case ItemType.Blessing:
                        upgradeItems.AddRange(blessings);
                        break;
                }
            }
            return RandomUtil.GetRandomItems(upgradeItems, count, GetUpgradeItemWeight);
        }

        int GetUpgradeItemWeight(Item item)
        {
            if (item.LVL.Value == item.MaxLVL) return 0;
            return GetItemWeight(item);
        }

        [Button("Load All Configs")]
        public void LoadAllConfigs()
        {
            ConfigManager.LoadAllConfigs();
            GetLists();
            ConfigData weaponAttributeConfig = ConfigManager.GetConfigData(PathDefines.WeaponAttribute);
            ConfigData weaponUpgradeConfig = ConfigManager.GetConfigData(PathDefines.WeaponUpgrade);
            foreach (var weapon in weapons)
            {
                weapon.LoadAttributeData(weaponAttributeConfig);
                weapon.LoadUpgradeData(weaponUpgradeConfig);
            }
        }
        
        
        [Button("Save All Configs")]
        public void SaveAllConfigs()
        {
            if (ConfigManager.configs.Count == 0)
            {
                ConfigManager.LoadAllConfigs(false);
                GetLists();
            }
            
            ConfigData weaponAttributeConfig = ConfigManager.GetConfigData(PathDefines.WeaponAttribute);
            ConfigData weaponUpgradeConfig = ConfigManager.GetConfigData(PathDefines.WeaponUpgrade);
            foreach (var weapon in weapons)
            {
                weapon.SaveAttributeData(weaponAttributeConfig);
                weapon.SaveUpgradeData(weaponUpgradeConfig);
            }
            ConfigManager.SaveAllConfigs();
        }
    }
}