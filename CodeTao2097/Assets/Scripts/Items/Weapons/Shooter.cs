using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public enum EAimWay
    {
        /// <summary>
        /// Shoot to the nearest target
        /// </summary>
        AutoTargeting,
        /// <summary>
        /// Shoot from the owner's moving direction
        /// </summary>
        Owner,
        /// <summary>
        /// Shoot to a random direction
        /// </summary>
        Random,
        /// <summary>
        /// Shoot to the cursor
        /// </summary>
        Cursor
    }
    
    public partial class Shooter : Weapon
    {
        protected BindableProperty<int> ammo = new BindableProperty<int>(3);
        public BindableProperty<int> maxAmmo = new BindableProperty<int>(3);
        public BindableProperty<float> reloadTime = new BindableProperty<float>(1);
        
        public Projectile projectilePrefab;
        protected ProjectilePool pool;
        
        /// <summary>
        /// A list of angles in degrees, that will be added to the base direction, and keep enumerating.
        /// </summary>
        public List<float> ShootingDirections = new List<float>();
        private int _currentDirectionIndex = 0;
        public float shootAmount = 1;
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
            _ownerMoveController = ComponentUtil.GetComponentInDescendants<MoveController>(unitController);
            ammo.Value = maxAmmo.Value;
            ammo.RegisterWithInitValue(value =>
            {
                if (value <= 0)
                {
                    StartReload();
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            pool = new ProjectilePool(projectilePrefab);
        }

        public override void Fire()
        {
            base.Fire();

            for (int i = 0; i < shootAmount; i++)
            {
                if (ammo.Value <= 0)
                {
                    break;
                }
                Vector2 direction = GetBaseDirection();
                if (ShootingDirections.Count > 0)
                {
                    _currentDirectionIndex += 1;
                    _currentDirectionIndex %= ShootingDirections.Count > 0 ? ShootingDirections.Count : 1;
                    float angle = Util.GetAngleFromVector(direction);
                    angle += ShootingDirections[_currentDirectionIndex];
                    direction = Util.GetVectorFromAngle(angle);
                }
                
                SpawnProjectile(direction);
            }
        }
        
        public virtual Projectile SpawnProjectile(Vector2 direction)
        {
            Projectile projectile = pool.Get();
            projectile.OnDestroy = () =>
            {
                pool.Release(projectile);
            };
            projectile.transform.position = transform.position + (Vector3)direction * shootPointOffset;
            projectile.transform.parent = ProjectileManager.Instance.transform;
            projectile.Init(this, direction);
            projectile.damager = damager;
            ammo.Value--;
            return projectile;
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
        
        public virtual void StartReload()
        {
            FireLoop.Pause();
            ActionKit.Delay(reloadTime.Value, () =>
            {
                Reload();
            }).Start(this);
        }
        
        public virtual void Reload()
        {
            ammo.Value = maxAmmo.Value;
            FireLoop.Resume(true);
        }
    }
}