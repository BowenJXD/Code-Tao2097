using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 近战武器，当攻击时，通过physics，对攻击范围内的所有单位造成伤害
    /// Melee weapon attacks in a certain range on fire.
    /// </summary>
    public class MeleeWeapon : Weapon
    {
        /// <summary>
        /// 0 - 360
        /// </summary>
        [BoxGroup("Melee")]
        public BindableStat angularRange = new BindableStat(0).SetMaxValue(360f);

        /// <summary>
        /// A list of angles in degrees, that will be added to the base direction, and keep enumerating.
        /// </summary>
        [BoxGroup("Melee")]
        public List<float> attackingDirections = new List<float>();
        private int _currentDirectionIndex = 0;
        
        [BoxGroup("Melee")]
        public EAimWay aimWay = EAimWay.Random;
        
        private MoveController _ownerMoveController;

        public override void OnAdd()
        {
            base.OnAdd();
            _ownerMoveController = Container.GetComp<MoveController>();
        }

        public override void Init()
        {
            base.Init();
            angularRange.AddModifierGroups(ats[EWAt.Area].ModGroups);
            attackRange.AddModifierGroups(ats[EWAt.Area].ModGroups);
        }
        
        public Action<float> onSpawn;
        
        public override void Fire()
        {
            base.Fire();
            
            float baseAngle = Util.GetAngleFromVector(GetBaseDirection());
            for (int i = 0; i < ats[EWAt.Amount].Value; i++)
            {
                float spawnAngle = baseAngle + GetSpawnAngle(i);
                onSpawn?.Invoke(spawnAngle);
                List<Defencer> targets = GetTargets();
                foreach (var target in targets)
                {
                    float targetAngle = Util.GetAngleFromVector(target.transform.position - transform.position);
                    if (Mathf.Abs(targetAngle - spawnAngle) < angularRange / 2)
                    {
                        Attack(target);
                    }
                }
            }
        }

        public float GetSpawnAngle(int spawnIndex)
        {
            float result = 0;
            if (attackingDirections.Count > 0)
            {
                _currentDirectionIndex += 1;
                _currentDirectionIndex %= attackingDirections.Count > 0 ? attackingDirections.Count : 1;
                result += attackingDirections[_currentDirectionIndex];
            }
            else
            {
                float amount = ats[EWAt.Amount].Value;
                result += spawnIndex * 360 / amount;
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

        [Button("Generate Directions")]
        public void GenerateDirections()
        {
            attackingDirections = Util.GenerateAngles((int)ats[EWAt.Amount], attackingDirections.FirstOrDefault());
            LogKit.I("Generated AttackingDirections: " + attackingDirections);
        }
    }
}