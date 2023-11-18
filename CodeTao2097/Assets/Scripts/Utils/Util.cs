﻿using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public static class Util
    {
        public static bool IsTagIncluded(string tag, List<ETag> tags)
        {
            foreach (var t in tags)
            {
                if (tag == t.ToString())
                {
                    return true;
                }
            }

            return false;
        }

        public static int GetRandomWeightedIndex(List<int> list)
        {
            int totalWeight = list.Sum();
            int randomValue = Global.Instance.Random.Next(totalWeight);

            for (int i = 0; i < list.Count; i++)
            {
                randomValue -= list[i];
                if (randomValue <= 0)
                {
                    return i;
                }
            }

            // This should not happen if the total weight is correct
            throw new InvalidOperationException("Unable to retrieve a random key.");
        }
    }

    public static class ComponentUtil
    {
        public static T GetComponentInSiblings<T>(Component component) where T : Component
        {
            Transform transform = component.transform;
            if (transform.parent == null)
            {
                return null;
            }

            Transform parent = transform.parent;
            foreach (Transform sibling in parent)
            {
                if (sibling != transform)
                {
                    T comp = sibling.GetComponent<T>();
                    if (comp != null)
                    {
                        return comp;
                    }
                }
            }

            return null;
        }

        public static T GetComponentInDescendants<T>(Component parent, int maxDepth = int.MaxValue) where T : Component
        {
            return GetComponentInDescendants<T>(parent.transform, 0, maxDepth);
        }

        private static T GetComponentInDescendants<T>(Transform parent, int currentDepth, int maxDepth) where T : Component
        {
            if (currentDepth > maxDepth)
            {
                return null;
            }

            foreach (Transform child in parent)
            {
                T component = child.GetComponent<T>();
            
                if (component != null)
                {
                    // Found the component, return it
                    return component;
                }
                
                // Avoid UnitController
                UnitController unitController = component as UnitController;
                if (unitController && !typeof(T).IsSubclassOf(typeof(UnitController)))
                {
                    return null;
                }


                // Recursively search the descendants with increased depth
                T descendantComponent = GetComponentInDescendants<T>(child, currentDepth + 1, maxDepth);
                if (descendantComponent != null)
                {
                    return descendantComponent;
                }
            }

            // Component not found within the specified depth
            return null;
        }
        
        public static T GetComponentInAncestors<T>(Component child, int maxDepth = int.MaxValue) where T : Component
        {
            return GetComponentInAncestors<T>(child.transform, 0, maxDepth);
        }

        private static T GetComponentInAncestors<T>(Transform child, int currentDepth, int maxDepth) where T : Component
        {
            if (currentDepth > maxDepth || child == null)
            {
                return null;
            }

            T component = child.GetComponent<T>();
            
            if (component != null)
            {
                // Found the component in ancestor, return it
                return component;
            }
            
            // Avoid UnitController
            UnitController unitController = component as UnitController;
            if (unitController && !typeof(T).IsSubclassOf(typeof(UnitController)))
            {
                return null;
            }

            // Recursively search the ancestors with increased depth
            return GetComponentInAncestors<T>(child.parent, currentDepth + 1, maxDepth);
        }
        
        public static string GetTagFromParent(Component component)
        {
            return component.transform.parent.tag;
        }
        
        public static UnitController GetUnitController(Component component)
        {
            return component.transform.parent.GetComponent<UnitController>();
        }
    }
}