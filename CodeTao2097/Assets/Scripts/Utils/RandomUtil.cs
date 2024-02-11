﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace CodeTao
{
    
    /// <summary>
    /// 各种与随机相关的工具集合
    /// </summary>
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
        
        /// <summary>
        /// Crit rate is between 0-100
        /// </summary>
        /// <param name="critRate"></param>
        /// <returns></returns>
        public static bool Rand100(float critRate)
        {
            return rand.Next(100) < critRate;
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
            for (int i = count - 1; i >= 0; i--)
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
}