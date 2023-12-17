using System;
using System.Collections.Generic;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeTao
{
    public class MeleeWeapon : Weapon
    {
        /// <summary>
        /// 0 - 360
        /// </summary>
        [BoxGroup("Secondary Attributes")]
        public BindableStat angularRange = new BindableStat(0).SetMaxValue(360f);

        /// <summary>
        /// angularRange = angularRange + area * areaAngularRatio
        /// </summary>
        public float angularAreaRatio = 15;
        public virtual float AngularRange { get => angularRange + ats[EWAt.Area] * angularAreaRatio; }
        
        /// <summary>
        /// attackRange = attackRange + area * areaRangeRatio
        /// </summary>
        public float areaRangeRatio = 1;
        public override float AttackRange { get => base.AttackRange + ats[EWAt.Area] * areaRangeRatio; }

        /// <summary>
        /// A list of angles in degrees, that will be added to the base direction, and keep enumerating.
        /// </summary>
        public List<float> attackingDirections = new List<float>();
        private int _currentDirectionIndex = 0;
        
        public EAimWay aimWay = EAimWay.Random;
        
        private MoveController _ownerMoveController;

        public override void OnAdd()
        {
            base.OnAdd();
            _ownerMoveController = ComponentUtil.GetComponentFromUnit<MoveController>(Container);
        }

        public override void Init()
        {
            base.Init();
        }

/*#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(MeleeWeapon))]
        public class MeleeWeaponEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
        
                if (GUILayout.Button("Generate Attacking Directions"))
                {
                    var controller = target as MeleeWeapon;
                    controller.GenerateShootingDirections();
                }
            }
        }
#endif
        
        public void GenerateShootingDirections()
        {
            attackingDirections = Util.GenerateAngles(ats[EWAt.Amount]);
            LogKit.I("Generated AttackingDirections: " + attackingDirections);
        }*/
        
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
                    if (Mathf.Abs(targetAngle - spawnAngle) < AngularRange / 2)
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
    }
}