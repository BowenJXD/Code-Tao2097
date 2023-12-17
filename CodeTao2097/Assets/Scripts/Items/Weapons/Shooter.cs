﻿using System;
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
        [BoxGroup("Shooter")]
        public List<float> ShootingDirections = new List<float>();
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
        
        public override Vector2 GetSpawnPoint(int spawnIndex)
        {
            Vector2 direction = GetBaseDirection();
            if (ShootingDirections.Count > 0)
            {
                _currentDirectionIndex += 1;
                _currentDirectionIndex %= ShootingDirections.Count > 0 ? ShootingDirections.Count : 1;
                float angle = Util.GetAngleFromVector(direction);
                angle += ShootingDirections[_currentDirectionIndex];
                direction = Util.GetVectorFromAngle(angle);
            }
            return direction * shootPointOffset;
        }

        public Vector2 GetBaseDirection()
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