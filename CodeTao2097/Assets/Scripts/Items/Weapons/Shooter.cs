using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public enum EShootDirectionBase
    {
        AutoTargeting,
        OwnerMoveDirection,
        Random,
        Cursor
    }
    
    public partial class Shooter : Weapon
    {
        protected BindableProperty<int> ammo = new BindableProperty<int>(3);
        public BindableProperty<int> maxAmmo = new BindableProperty<int>(3);
        public BindableProperty<float> reloadTime = new BindableProperty<float>(1);
        
        public Projectile projectilePrefab;
        public ProjectilePool pool;

        public List<float> ShootingDirections = new List<float>();
        private int _currentDirectionIndex = 0;
        public float shootAmount = 1;
        public float shootPointOffset = 1;
        public EShootDirectionBase shootDirectionBase = EShootDirectionBase.Random;
        
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
            projectile.transform.parent = transform;
            projectile.Init(this, direction);
            projectile.damager = damager;
            ammo.Value--;
            return projectile;
        }

        public Vector2 GetBaseDirection()
        {
            Vector2 result = Util.GetRandomNormalizedVector();
            switch (shootDirectionBase)
            {
                case EShootDirectionBase.AutoTargeting:
                    if (GetTargets().Count > 0)
                    {
                        Defencer target = GetTargets()[0];
                        result = (target.transform.position - transform.position);
                    }
                    break;
                case EShootDirectionBase.OwnerMoveDirection:
                    result = _ownerMoveController.LastNonZeroDirection.Value;
                    break;
                case EShootDirectionBase.Random:
                    result = Util.GetRandomNormalizedVector();
                    break;
                case EShootDirectionBase.Cursor:
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