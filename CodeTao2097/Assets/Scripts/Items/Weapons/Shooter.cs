using System;
using System.Collections.Generic;
using System.Linq;
using CodeTao;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 生成直线飞行的弹射物
    /// Spawns projectiles that fly in a straight line.
    /// </summary>
    public partial class Shooter : SpawnerWeapon<Projectile>
    {
        [BoxGroup("Shooter")]
        public BindableStat penetration = new BindableStat(1);
        [BoxGroup("Shooter")]
        public float shootPointOffset = 1;
        [BoxGroup("Shooter")]
        public EAimWay aimWay = EAimWay.Random;
        /// <summary>
        /// A list of angles in degrees, that will be added to the base direction, and keep enumerating.
        /// </summary>
        [FormerlySerializedAs("ShootingDirections")] [BoxGroup("Shooter")]
        public List<float> shootingDirections = new List<float>();
        private int _currentDirectionIndex = 0;
        
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

        private void OnEnable()
        {
            damager = ShooterDamager;
        }

        public override Projectile SpawnUnit(Vector2 localPos)
        {
            Projectile unit = base.SpawnUnit(localPos);
            unit.Parent(ProjectileManager.Instance.transform)
                .Rotation(Quaternion.Euler(0, 0, Util.GetAngleFromVector(localPos.normalized)))
                .LocalScale(new Vector3(ats[EWAt.Area], ats[EWAt.Area]))
                .SetWeapon(this) // and damager
                .SetPenetration(penetration)
                .SetLifeTime(ats[EWAt.Duration])
                .SetSPD(ats[EWAt.Speed])
                .SetMovingDirection(localPos.normalized);
            return unit;
        }
        
        public override Vector2 GetLocalSpawnPoint(Vector2 basePoint, int spawnIndex)
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

        public override void ModAttribute(WeaponUpgradeMod mod)
        {
            base.ModAttribute(mod);
            if (mod.attribute == EWAt.Penetration)
            {
                penetration.AddModifier(mod.value, mod.modType, $"Level{LVL + 1}");
            }
        }
        
        [Button("Generate Attacking Directions")]
        public void GenerateDirections()
        {
            shootingDirections = Util.GenerateAngles((int)ats[EWAt.Amount], shootingDirections.FirstOrDefault());
            LogKit.I("Generated AttackingDirections: " + shootingDirections);
        }
    }
}