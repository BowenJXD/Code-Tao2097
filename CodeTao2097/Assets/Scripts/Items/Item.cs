﻿using System;
using System.Collections.Generic;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeTao
{
    public abstract class Item : Content<Item>
    {
        [TabGroup("Item")]
        public BindableStat weight = new BindableStat(10);
        [TabGroup("Item")]
        public List<ElementType> relatedElements = new List<ElementType>();

        public override void OnAdd()
        {
            base.OnAdd();
            Init();
        }

        public virtual void Init()
        {
            gameObject.SetActive(true);
        }
        
        public virtual int GetWeight()
        {
            return (int)weight.Value;
        }

        public override void Upgrade(int lvlIncrement = 1)
        {
            base.Upgrade(lvlIncrement);
            int newLevel = LVL.Value;
            if (newLevel == MaxLVL)
            {
                weight.Value = 0;
            }
        }

        public virtual string GetUpgradeDescription()
        {
            int newLevel = LVL.Value + 1;
            return $"{(newLevel == 1 ? "New!! " : "")} {GetType().Name} ({newLevel}): ";
        }
    }
}