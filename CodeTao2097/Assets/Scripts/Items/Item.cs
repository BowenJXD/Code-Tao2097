using System;
using System.Collections.Generic;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeTao
{
    public enum ItemType
    {
        Artefact,
        Weapon,
        Blessing
    }
    
    /// <summary>
    /// 物品基类，包含物品的基本属性，以及物品的基本功能
    /// </summary>
    public abstract class Item : Content<Item>
    {
        [BoxGroup("Item")]
        public BindableStat weight = new BindableStat(10);
        [BoxGroup("Item")]
        public List<ElementType> relatedElements = new List<ElementType>();

        public override void OnAdd()
        {
            base.OnAdd();
            Init();
        }

        public virtual void Init()
        {
            gameObject.SetActive(true);
            weight.Init();
        }
        
        public virtual int GetWeight()
        {
            return (int)weight;
        }

        public override void AlterLVL(int lvlIncrement = 1)
        {
            base.AlterLVL(lvlIncrement);
            int newLevel = LVL.Value;
            if (newLevel == MaxLVL)
            {
                weight.Value = 0;
            }
        }

        public virtual string GetUpgradeDescription()
        {
            int newLevel = LVL.Value + 1;
            return $"{(newLevel == 1 ? "New!! " : "")} {name} ({newLevel}): ";
        }

        private void OnDisable()
        {
            weight.Reset();
        }
    }
}