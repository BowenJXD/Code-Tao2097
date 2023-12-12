using System.Collections.Generic;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeTao
{
    public partial class Orbiter : SpawnerWeapon<Projectile>
    {
        /// <summary>
        /// A list of angles in degrees, that will be added to the base direction, and keep enumerating.
        /// </summary>
        public List<float> ShootingDirections = new List<float>();
        private int _currentDirectionIndex = 0;

        protected void OnEnable()
        {
            WheelJoint2D wheelJoint2D = GetComponent<WheelJoint2D>();
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            
            if (wheelJoint2D)
            {
                JointMotor2D motor = wheelJoint2D.motor;
                motor.motorSpeed = ats[EWAt.Speed].Value;
                wheelJoint2D.motor = motor;
                ats[EWAt.Speed].RegisterWithInitValue(value =>
                {
                    JointMotor2D motor = wheelJoint2D.motor;
                    motor.motorSpeed = value;
                    wheelJoint2D.motor = motor;
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
            else if (rb2D)
            {
                rb2D.angularVelocity = ats[EWAt.Speed].Value;
                ats[EWAt.Speed].RegisterWithInitValue(value => rb2D.angularVelocity = value)
                    .UnRegisterWhenGameObjectDestroyed(this);
            }
        }

        [Button]
        public void GenerateShootingDirections()
        {
            ShootingDirections.Clear();
            int count = ats[EWAt.Amount];
            float angle = 360f / count;
            for (int i = 0; i < count; i++)
            {
                ShootingDirections.Add(angle * i);
            }
            LogKit.I("Generated ShootingDirections: " + ShootingDirections);
        }
        
        public override Projectile SpawnUnit(Vector2 spawnPosition)
        {
            Projectile unit = base.SpawnUnit(spawnPosition);
            unit.lifeTime.Value = ats[EWAt.Duration].Value;
            unit.Parent(transform)
                .SetMovingDirection(Vector2.zero)
                .Rotation(Quaternion.Euler(0, 0, Util.GetAngleFromVector(spawnPosition.normalized)))
                .Init(this);
            return unit;
        }
        
        public override Vector2 GetSpawnPoint(int spawnIndex)
        {
            Vector2 direction = Vector2.zero;
            if (ShootingDirections.Count > 0)
            {
                _currentDirectionIndex += 1;
                _currentDirectionIndex %= ShootingDirections.Count > 0 ? ShootingDirections.Count : 1;
                float angle = Util.GetAngleFromVector(direction);
                angle += ShootingDirections[_currentDirectionIndex];
                direction = Util.GetVectorFromAngle(angle);
            }
            return direction * ats[EWAt.Area];
        }
    }
}