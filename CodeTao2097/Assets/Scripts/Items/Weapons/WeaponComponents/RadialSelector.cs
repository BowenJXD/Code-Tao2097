using System.Collections.Generic;
using System.Linq;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 角度选择器，基于角度生成攻击的位置信息。
    /// </summary>
    public class RadialSelector : WeaponSelector
    {
        public EAimWay aimWay = EAimWay.Random;
        public float shootPointOffset = 1;
        /// <summary>
        /// A list of angles in degrees, that will be added to the base direction, and keep enumerating.
        /// </summary>
        public List<float> shootingDirections = new List<float>();
        private int _currentDirectionIndex = 0;
        
        private MoveController _ownerMoveController;

        public override void Init(Weapon weapon)
        {
            base.Init(weapon);
            _ownerMoveController = weapon.Container.GetComp<MoveController>();
        }
        
        public override List<Vector3> GetGlobalPositions()
        {
            int amount = (int)weapon.amount.Value;
            List<Vector3> results = new List<Vector3>();
            for (int i = 0; i < amount; i++)
            {
                Vector3 result = transform.position + GetLocalPosition(i);
                results.Add(result);
            }

            return results;
        }

        public Vector3 GetLocalPosition(int spawnIndex)
        {
            Vector2 basePoint = GetBaseDirection();
            float angle = Util.GetAngleFromVector(basePoint);
            if (shootingDirections.Count > 0)
            {
                _currentDirectionIndex += 1;
                _currentDirectionIndex %= shootingDirections.Count > 0 ? shootingDirections.Count : 1;
                angle += shootingDirections[_currentDirectionIndex];
            }
            else
            {
                float amount = weapon.amount.Value;
                angle += spawnIndex * 360 / amount;
            }
            
            return Util.GetVectorFromAngle(angle) * shootPointOffset;
        }

        public Vector2 GetBaseDirection()
        {
            Vector2 result = RandomUtil.GetRandomNormalizedVector();
            switch (aimWay)
            {
                case EAimWay.AutoTargeting:
                    var targets = weapon.GetTargets();
                    if (targets.Count > 0)
                    {
                        Defencer target = targets[0];
                        result = (target.transform.position - transform.position);
                    }
                    break;
                case EAimWay.Owner:
                    result = _ownerMoveController.lastNonZeroDirection.Value;
                    break;
                case EAimWay.Random:
                    result = RandomUtil.GetRandomNormalizedVector();
                    break;
                case EAimWay.Cursor:
                    result = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
                    break;
                default:
                    result = Vector2.right;
                    break;
            }

            return result.normalized;
        }
        
        [Button("Generate Directions")]
        public void GenerateDirections()
        {
            if (!weapon) weapon = GetComponent<Weapon>();
            if (!weapon) weapon = this.GetComponentInAncestors<Weapon>(1);
            shootingDirections = Util.GenerateAngles((int)weapon.amount, shootingDirections.FirstOrDefault());
            LogKit.I("Generated AttackingDirections: " + shootingDirections);
        }
    }
}