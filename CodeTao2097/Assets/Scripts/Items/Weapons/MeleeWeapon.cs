using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    public class MeleeWeapon : Weapon
    {
        /// <summary>
        /// 0 - 360
        /// </summary>
        public BindableStat angularRange = new BindableStat(1).SetMaxValue(360f);
        /// <summary>
        /// A list of angles in degrees, that will be added to the base direction, and keep enumerating.
        /// </summary>
        public List<float> attackingDirections = new List<float>();
        private int _currentDirectionIndex = 0;
        
        public EAimWay aimWay = EAimWay.Random;
        
        private MoveController _ownerMoveController;

        private void Awake()
        {
            AddAfter += content =>
            {
                _ownerMoveController = ComponentUtil.GetComponentFromUnit<MoveController>(Container);
            };
        }

        public override void Fire()
        {
            base.Fire();
            
            for (int i = 0; i < ats[EWAt.Amount].Value; i++)
            {
                float spawnAngle = GetSpawnAngle(i);
                ProcessSpawnAngle(spawnAngle);
                List<Defencer> targets = GetTargets(ats[EWAt.Area]);
                foreach (var target in targets)
                {
                    float targetAngle = Util.GetAngleFromVector(target.transform.position - transform.position);
                    if (Mathf.Abs(targetAngle - spawnAngle) < angularRange.Value / 2)
                    {
                        Attack(target);
                    }
                }
            }
        }

        public void ProcessSpawnAngle(float spawnAngle)
        {
            
        }

        public float GetSpawnAngle(int spawnIndex)
        {
            float result = Util.GetAngleFromVector(GetBaseDirection());
            if (attackingDirections.Count > 0)
            {
                _currentDirectionIndex += 1;
                _currentDirectionIndex %= attackingDirections.Count > 0 ? attackingDirections.Count : 1;
                result += attackingDirections[_currentDirectionIndex];
            }
            return result;
        }
        
        public Vector2 GetBaseDirection()
        {
            Vector2 result = RandomUtil.GetRandomNormalizedVector();
            switch (aimWay)
            {
                case EAimWay.AutoTargeting:
                    var targets = GetTargets(ats[EWAt.Area]);
                    if (targets.Count > 0)
                    {
                        Defencer target = targets[0];
                        result = (target.transform.position - transform.position);
                    }
                    break;
                case EAimWay.Owner:
                    result = _ownerMoveController.LastNonZeroDirection.Value;
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
    }
}