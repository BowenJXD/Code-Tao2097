using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;

namespace CodeTao
{
    public class ElementSystem : MonoSingleton<ElementSystem>
    {
        public int maxElements = 3;
        public List<ElementType> elements;
        
        private Inventory inventory;

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            if (!inventory) inventory = Player.Instance.Inventory;
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
            
            
            
            // if the related elements has a NONE element, the weight will not be deducted to 0
            if (relatedElements.Contains(ElementType.None))
            {
                weight = 1;
            }
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
                // the weight will be deducted to 0 if the related elements can not be all selected (selected is full)
                if (elements.Count >= maxElements
                    && !relatedElements.All(element => elements.Contains(element)))
                {
                    weight = 0;
                }
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
            inventory.AddAfter -= OnInventoryAddItem;
        }
    }
}