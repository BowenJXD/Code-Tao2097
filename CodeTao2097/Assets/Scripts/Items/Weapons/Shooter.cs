using System;
using System.Collections.Generic;
using CodeTao;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public partial class Shooter : SpawnerWeapon<Projectile>
    {
        [BoxGroup("Secondary Attributes")]
        public BindableProperty<int> penetration = new BindableProperty<int>(1);
        
        /// <summary>
        /// A list of angles in degrees, that will be added to the base direction, and keep enumerating.
        /// </summary>
        [FormerlySerializedAs("ShootingDirections")] [BoxGroup("Shooter")]
        public List<float> shootingDirections = new List<float>();
        private int _currentDirectionIndex = 0;
        [BoxGroup("Shooter")]
        public float shootPointOffset = 1;
        [BoxGroup("Shooter")]
        public EAimWay aimWay = EAimWay.Random;
        
        private MoveController _ownerMoveController;

        public override void OnAdd()
        {
            base.OnAdd();
            _ownerMoveController = ComponentUtil.GetComponentFromUnit<MoveController>(Container);
        }

        private void Awake()
        {
            damager = ShooterDamager;
        }

/*#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(Shooter))]
        public class ShooterEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
        
                if (GUILayout.Button("Generate Spawning Directions"))
                {
                    var controller = target as Shooter;
                    controller.GenerateShootingDirections();
                }
            }
        }
#endif
        
        public void GenerateShootingDirections()
        {
            shootingDirections = Util.GenerateAngles(ats[EWAt.Amount]);
            LogKit.I("Generated SpawningDirections: " + shootingDirections);
        }*/
        
        public override Projectile SpawnUnit(Vector2 spawnPosition)
        {
            Projectile unit = base.SpawnUnit(spawnPosition);
            unit.lifeTime.Value = ats[EWAt.Duration].Value;
            unit.penetration = penetration;
            unit.Parent(ProjectileManager.Instance.transform)
                .SetMovingDirection(spawnPosition.normalized)
                .Rotation(Quaternion.Euler(0, 0, Util.GetAngleFromVector(spawnPosition.normalized)))
                .LocalScale(new Vector3(ats[EWAt.Area], ats[EWAt.Area]))
                .Init(this);
            return unit;
        }
        
        public override Vector2 GetSpawnPoint(Vector2 basePoint, int spawnIndex)
        {
            float angle = Util.GetAngleFromVector(basePoint);
            if (shootingDirections.Count > 0)
            {
                _currentDirectionIndex += 1;
                _currentDirectionIndex %= shootingDirections.Count > 0 ? shootingDirections.Count : 1;
                angle += shootingDirections[_currentDirectionIndex];
            }
            else
            {
                float amount = ats[EWAt.Amount].Value;
                angle += spawnIndex * 360 / amount;
            }
            
            return Util.GetVectorFromAngle(angle) * shootPointOffset;
        }

        public override Vector2 GetBaseDirection()
        {
            Vector2 result = RandomUtil.GetRandomNormalizedVector();
            switch (aimWay)
            {
                case EAimWay.AutoTargeting:
                    var targets = GetTargets();
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