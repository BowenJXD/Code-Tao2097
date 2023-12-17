using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;
using Random = System.Random;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        /// <returns>Angle in degrees 0 - 360</returns>
        public static float GetAngleFromVector(Vector2 dir)
        {
            return Vector2.SignedAngle(Vector2.right, dir);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>Normalized vector derived from angle (from positive x axis)</returns>
        public static Vector2 GetVectorFromAngle(float angle)
        {
            return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }

        public static Vector2 AddAnglesInV2(Vector2 v1, Vector2 v2)
        {
            float angle1 = GetAngleFromVector(v1);
            float angle2 = GetAngleFromVector(v2);
            float angle = angle1 + angle2;
            return GetVectorFromAngle(angle);
        }
        
        /// <summary>
        /// Find the 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="vector2s"></param>
        /// <returns></returns>
        public static Vector2 FindNearestV2WithAngle(Vector2 target, Vector2 source, List<Vector2> vector2s)
        {
            Vector2 result = Vector2.zero;
            float minAngle = float.MaxValue;
            foreach (var vector2 in vector2s)
            {
                float angle = Vector2.Angle(target - source, vector2 - source);
                if (angle < minAngle)
                {
                    minAngle = angle;
                    result = vector2;
                }
            }

            return result;
        }
    }

    public static class RandomUtil
    {
        public static Random rand => Global.Instance.Random;
        
        public static int GetRandomWeightedIndex(List<int> list)
        {
            int totalWeight = list.Sum();
            int randomValue = rand.Next(totalWeight);

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
        
        /// <summary>
        /// Get random items from list without repeating
        /// </summary>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <param name="getWeight"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetRandomItems<T>(List<T> list, int count, Func<T, int> getWeight)
        {
            List<T> listCache = new List<T>(list);
            count = Mathf.Clamp(count, 0, listCache.Count);
            List<T> result = new List<T>();
            List<int> weights = listCache.Select(getWeight).ToList();
            for (int i = 0; i < count; i++)
            {
                int randomIndex = GetRandomWeightedIndex(weights);
                result.Add(listCache[randomIndex]);
                weights.RemoveAt(randomIndex);
                listCache.RemoveAt(randomIndex);
            }

            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandomScreenPosition(float marginPercentage = 0.1f)
        {
            // Assuming you're using the main camera
            Camera mainCamera = Camera.main;
            
            marginPercentage = Mathf.Clamp(marginPercentage, 0, 1);
            
            // Generate random x and y coordinates within the screen boundaries
            float randomX = rand.Next((int)(Screen.width * (1 - marginPercentage)));
            float randomY = rand.Next((int)(Screen.height * (1 - marginPercentage)));

            // Convert screen coordinates to world coordinates
            Vector3 randomScreenPosition = new Vector3(randomX, randomY, 10f); // 10f is the distance from the camera

            // Convert screen position to world position
            Vector3 randomWorldPosition = mainCamera.ScreenToWorldPoint(randomScreenPosition);

            return randomWorldPosition;
        }

        public static Vector2 GetRandomNormalizedVector()
        {
            float randomAngle = Global.Instance.Random.Next(360);
            Vector2 randomDirection = Util.GetVectorFromAngle(randomAngle);
        
            return randomDirection;
        }
    }

    public static class ComponentUtil
    {
        public static T GetComponentFromUnit<T>(Component component) where T : Component
        {
            UnitController unitController = GetComponentInAncestors<UnitController>(component);
            if (unitController)
            {
                T result = GetComponentInDescendants<T>(unitController);
                if (result)
                {
                    return result;
                }
            }
            return null;
        }
        
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
        
        /// <summary>
        /// /// Get component in descendants, including self, stop when maxDepth or UnitController is reached
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="maxDepth"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
                
                // Stop with UnitController unless the searching component is a subclass of UnitController
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
        
        /// <summary>
        /// Get components in descendants, including self, stop when maxDepth or UnitController is reached
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="maxDepth"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetComponentsInDescendants<T>(Component parent, int maxDepth = int.MaxValue) where T : Component
        {
            List<T> components = new List<T>();
            GetComponentsInDescendants(parent.transform, components, 0, maxDepth);
            return components;
        }

        private static void GetComponentsInDescendants<T>(Transform parent, List<T> components, int currentDepth, int maxDepth) where T : Component
        {
            if (currentDepth > maxDepth)
            {
                return;
            }

            foreach (Transform child in parent)
            {
                T component = child.GetComponent<T>();

                if (component != null)
                {
                    // Found a component, add it to the list
                    components.Add(component);
                }

                // Stop with UnitController unless the searching component is a subclass of UnitController
                UnitController unitController = component as UnitController;
                if (unitController && !typeof(T).IsSubclassOf(typeof(UnitController)))
                {
                    return;
                }

                // Recursively search the descendants with increased depth
                GetComponentsInDescendants(child, components, currentDepth + 1, maxDepth);
            }
        }
        
        /// <summary>
        /// Get component in ancestors, including self, stop when maxDepth or UnitController is reached
        /// </summary>
        /// <param name="child"></param>
        /// <param name="maxDepth"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
            
            // Stop with UnitController unless the searching component is a subclass of UnitController
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
            return GetComponentInAncestors<UnitController>(component);
        }
    }
}