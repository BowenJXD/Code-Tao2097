using System;
using System.Collections.Generic;
using CodeTao;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public partial class Shooter : SpawnerWeapon<Projectile>
    {
        /// <summary>
        /// A list of angles in degrees, that will be added to the base direction, and keep enumerating.
        /// </summary>
        public List<float> ShootingDirections = new List<float>();
        private int _currentDirectionIndex = 0;
        public float shootPointOffset = 1;
        public EAimWay aimWay = EAimWay.Random;
        
        private MoveController _ownerMoveController;

        private void Awake()
        {
            damager = ShooterDamager;
        }

        protected override void Start()
        {
            base.Start();
            
            UnitController unitController = ComponentUtil.GetComponentInAncestors<UnitController>(this);
        }

        public override Projectile SpawnUnit(Vector2 spawnPosition)
        {
            Projectile unit = base.SpawnUnit(spawnPosition);
            unit.transform.parent = ProjectileManager.Instance.transform;
            unit.lifeTime.Value = ats[EWAt.Duration].Value;
            unit.Init(this, spawnPosition.normalized);
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
            Vector2 result = Util.GetRandomNormalizedVector();
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
                    result = Util.GetRandomNormalizedVector();
                    break;
                case EAimWay.Cursor:
                    result = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
                    break;
            }

            return result.normalized;
        }
        
        public override void Upgrade(int lvlIncrement = 1)
        {
            base.Upgrade(lvlIncrement);
            switch (LVL.Value)
            {
                default:
                    damager.KnockBackFactor.AddModifier($"Level{LVL.Value}", 1, EModifierType.Additive, ERepetitionBehavior.AddStack);
                    break;
            }
        }
    }
}