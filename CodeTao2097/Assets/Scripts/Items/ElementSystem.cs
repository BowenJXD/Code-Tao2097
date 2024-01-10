using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;

namespace CodeTao
{
    /// <summary>
    /// 玩家的元素系统，用于记录玩家拥有的元素，并基于现有元素控制池子中物品的权重
    /// </summary>
    public class ElementSystem : MonoSingleton<ElementSystem>
    {
        public int maxElements = 3;
        public List<ElementType> elements;
        
        private Inventory inventory;

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            if (!inventory) inventory = Player.Instance.Link.GetComp<Inventory>();
            inventory.AddAfter += OnInventoryAddItem;
        }

        void OnInventoryAddItem(Content<Item> newItem)
        {
            Weapon weapon = newItem as Weapon;
            if (weapon && !elements.Contains(weapon.elementType))
            {
                elements.Add(weapon.elementType);
            }
        }

        public int GetElementWeight(List<ElementType> relatedElements)
        {
            int weight = 1;
            
            // 如果相关元素中有NONE元素，权重不会降为0
            // if the related elements has a NONE element, the weight will not be deducted to 0
            if (relatedElements.Contains(ElementType.None))
            {
                weight = 1;
            }
            // 如果相关元素中有ALL元素，且相关元素没有全部被选中，权重会降为0
            // if the related elements has a ALL element, the weight will always be deducted to 0 if the related elements are not all selected
            else if (relatedElements.Contains(ElementType.All))
            {
                weight = 1;
                if (!relatedElements.All(element => elements.Contains(element) || element == ElementType.All))
                {
                    weight = 0;
                }
            }
            else
            {
                // 如果相关元素中有任意一个元素没有被选中，且所选元素满了，则权重会降为0
                // the weight will be deducted to 0 if the related elements can not be all selected (selected is full)
                if (elements.Count >= maxElements
                    && !relatedElements.All(element => elements.Contains(element)))
                {
                    weight = 0;
                }
                // 如果相关元素中有任意一个元素没有被选中，则权重会翻倍
                // the weight will be doubled for each related element that is selected
                else
                {
                    foreach (ElementType element in relatedElements)
                    {
                        if (elements.Contains(element))
                        {
                            weight *= 2;
                        }
                    }
                }
            }
            return weight;
        }

        private void OnDisable()
        {
            if (inventory) inventory.AddAfter -= OnInventoryAddItem;
        }
    }
}