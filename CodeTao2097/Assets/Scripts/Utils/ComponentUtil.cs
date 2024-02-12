using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    
    /// <summary>
    /// 各种与组件相关的工具集合，适用于多层级的组件结构
    /// </summary>
    public static class ComponentUtil
    {
        /// <summary>
        /// Get component from unit, including the unit controller. Stop when UnitController is reached
        /// </summary>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetComponentFromUnit<T>(this Component component) where T : Component
        {
            UnitController unitController = component is UnitController ? (UnitController)component : GetComponentInAncestors<UnitController>(component);
            if (unitController)
            {
                if (unitController.TryGetComponent<T>(out T result))
                {
                    return result;
                }
                result = GetComponentInDescendants<T>(unitController);
                if (result)
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// /// Get component in descendants, excluding self, stop when maxDepth or UnitController is reached
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="inactive"></param>
        /// <param name="layer"></param>
        /// <param name="maxDepth"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetComponentInDescendants<T>(this Component parent, bool inactive = false, int layer = 0, int maxDepth = int.MaxValue) where T : Component
        {
            if (parent == null)
            {
                return null;
            }
            return GetComponentInDescendants<T>(parent.transform, 0, maxDepth, inactive, layer);
        }

        private static T GetComponentInDescendants<T>(Transform parent, int currentDepth, int maxDepth, bool inactive = false, int layer = 0) where T : Component
        {
            // if maxDepth is reached, return null
            if (currentDepth > maxDepth)
            {
                return null;
            }
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                // if inactive is false, only search the active game objects
                if (!inactive && child.gameObject.activeSelf == false)
                {
                    continue;
                }
                // if layer is specified, only search the game objects in the specified layer
                if (layer != 0 && child.gameObject.layer != layer)
                {
                    continue;
                }
                // if component is found, return the component
                if (child.TryGetComponent<T>(out T component))
                {
                    return component;
                }
                // if stopper is found, return to the last level
                if (child.TryGetComponent<IStopper>(out IStopper _))
                {
                    continue;
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

        /// <summary>
        /// Get components in descendants, excluding self, stop when maxDepth or UnitController is reached
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="inactive">True if want to consider inactive game objects</param>
        /// <param name="maxDepth"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetComponentsInDescendants<T>(this Component parent, bool inactive = false, int maxDepth = int.MaxValue)
        {
            List<T> components = new List<T>();
            GetComponentsInDescendants(parent.transform, components, inactive, 0, maxDepth);
            return components;
        }

        private static void GetComponentsInDescendants<T>(Transform parent, List<T> components, bool inactive, int currentDepth, int maxDepth)
        {
            // if maxDepth is reached, return
            if (currentDepth > maxDepth)
            {
                return;
            }
            foreach (Transform child in parent)
            {
                // if inactive is false, only search the active game objects
                if (child.gameObject.activeSelf == false && !inactive)
                {
                    continue;
                }
                // if component is found, add the component to the list
                if (child.TryGetComponent<T>(out T component) )
                {
                    components.Add(component);
                }
                // if stopper is found, return to the last level
                if (child.TryGetComponent<IStopper>(out IStopper _))
                {
                    continue;
                }
                // Recursively search the descendants with increased depth
                GetComponentsInDescendants(child, components, inactive, currentDepth + 1, maxDepth);
            }
        }
        
        /// <summary>
        /// Get component in ancestors, including self, stop when maxDepth or UnitController is reached
        /// </summary>
        /// <param name="child"></param>
        /// <param name="maxDepth"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetComponentInAncestors<T>(this Component child, int maxDepth = int.MaxValue) where T : Component
        {
            if (child == null)
            {
                return null;
            }
            return GetComponentInAncestors<T>(child.transform, 0, maxDepth);
        }

        private static T GetComponentInAncestors<T>(Transform child, int currentDepth, int maxDepth) where T : Component
        {
            // if maxDepth is reached, return null
            if (currentDepth > maxDepth || child == null)
            {
                return null;
            }
            // if component is found, return the component
            if (child.TryGetComponent<T>(out T component))
            {
                return component;
            }
            // if stopper is found, return to the last level
            if (child.TryGetComponent<IStopper>(out IStopper _))
            {
                return null;
            }
            // Recursively search the ancestors with increased depth
            return GetComponentInAncestors<T>(child.parent, currentDepth + 1, maxDepth);
        }
        
        public static T GetComp<T>(this Collider2D col) where T : UnitComponent
        {
            UnitController unitController = ColliderManager.Instance.colUnitDict[col];
            return unitController.GetComp<T>();
        }

        public static UnitController GetUnit(this Collider2D col)
        {
            return ColliderManager.Instance.colUnitDict[col];
        }

        public static Collider2D GetCollider(this UnitController unit, int layer = 0)
        {
            Collider2D col = ColliderManager.Instance.GetCollider(unit, layer);
            return col;
        }
    }
}