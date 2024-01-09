using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;
using UnityEngine.Timeline;
using Random = System.Random;

namespace CodeTao
{
    public static class Util
    {
        public static bool IsTagIncluded(string tag, List<EntityType> tags)
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
        
        public static List<float> GenerateAngles(int count, float step)
        {
            List<float> result = new List<float>();
            float start = step * (count - 1) / 2;
            for (int i = 0; i < count; i++)
            {
                result.Add(start - i * step);
            }

            return result;
        }

        public static Color GetColor(this ElementType type)
        {
            switch (type)
            {
                case ElementType.Metal:
                    return Color.yellow;
                case ElementType.Wood:
                    return Color.green;
                case ElementType.Water:
                    return Color.blue;
                case ElementType.Fire:
                    return Color.red;
                case ElementType.Earth:
                    return new Color(165,42,42);
                default:
                    return Color.white;
            }
        }
    }

    public static class RandomUtil
    {
        public static Random rand => Global.Instance.Random;
        
        public static int Rand(int max)
        {
            return rand.Next(Mathf.Abs(max));
        }
        
        public static bool RandBool()
        {
            return rand.Next(2) == 0;
        }
        
        public static int RandRange(int a, int b)
        {
            int min = Mathf.Min(a, b);
            int max = Mathf.Max(a, b);
            return rand.Next(min, max);
        }
        
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
        /// <returns>A random relative position from the camera</returns>
        public static Vector3 GetRandomScreenPosition(float marginPercentage = 0.1f)
        {
            Camera mainCamera = Camera.main;
            float cameraHeight = 2 * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;
            
            marginPercentage = Mathf.Clamp(1 - marginPercentage, 0, 1);
            
            float randomX = ((float)rand.NextDouble() * cameraWidth - cameraWidth / 2) * marginPercentage;
            float randomY = ((float)rand.NextDouble() * cameraHeight - cameraHeight / 2) * marginPercentage;

            Vector3 randomWorldPosition = new Vector3(randomX, randomY, 10f); // 10f is the distance from the camera
            // Vector3 randomWorldPosition = mainCamera.ScreenToWorldPoint(randomScreenPosition);

            return randomWorldPosition;
        }

        public static Vector2 GetRandomNormalizedVector()
        {
            float randomAngle = rand.Next(360);
            Vector2 randomDirection = Util.GetVectorFromAngle(randomAngle);
        
            return randomDirection;
        }
    }

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
                T result;
                result = unitController.GetComponent<T>();
                if (result)
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
        
        public static T GetComponentInSiblings<T>(this Component component) where T : Component
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
            if (currentDepth > maxDepth)
            {
                return null;
            }
            
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                
                if (!inactive && child.gameObject.activeSelf == false)
                {
                    continue;
                }
                
                if (layer != 0 && child.gameObject.layer != layer)
                {
                    continue;
                }
                
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
        /// Get components in descendants, excluding self, stop when maxDepth or UnitController is reached
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="inactive">True if want to consider inactive game objects</param>
        /// <param name="maxDepth"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetComponentsInDescendants<T>(this Component parent, bool inactive = false, int maxDepth = int.MaxValue) where T : Component
        {
            List<T> components = new List<T>();
            GetComponentsInDescendants(parent.transform, components, inactive, 0, maxDepth);
            return components;
        }

        private static void GetComponentsInDescendants<T>(Transform parent, List<T> components, bool inactive, int currentDepth, int maxDepth) where T : Component
        {
            if (currentDepth > maxDepth)
            {
                return;
            }

            foreach (Transform child in parent)
            {
                if (child.gameObject.activeSelf == false && !inactive)
                {
                    continue;
                }
                
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
        
        public static string GetTagFromParent(this Component component)
        {
            return component.transform.parent.tag;
        }
        
        public static UnitController GetUnitController(this Component component)
        {
            return GetComponentInAncestors<UnitController>(component);
        }
    }
}