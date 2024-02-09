using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;
using UnityEngine.Timeline;
using Random = System.Random;

namespace CodeTao
{
    /// <summary>
    /// 各种工具集合
    /// </summary>
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

        public static float GetScale(this Vector3 vector)
        {
            // use average
            return (vector.x + vector.y + vector.z) / 3;
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
                    return new Color(0.3f, 0.18f, 0.09f);
                default:
                    return Color.white;
            }
        }
        
        public static Collider2D[] GetVisibleColliders(int layerMask = 0)
        {
            Camera mainCamera = Camera.main;
            Vector3 cameraPosition = mainCamera.transform.position;
            float cameraHeight = 2 * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;
            Vector3 cameraSize = new Vector3(cameraWidth, cameraHeight, 0);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(cameraPosition, cameraSize, 0);
            return colliders;
        }
    }
    
}