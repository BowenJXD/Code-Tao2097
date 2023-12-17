using System.Collections.Generic;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public partial class Orbiter : SpawnerWeapon<Projectile>
    {
        /// <summary>
        /// A list of angles in degrees, that will be added to the base direction, and keep enumerating.
        /// </summary>
        [FormerlySerializedAs("ShootingDirections")] public List<float> spawningDirections = new List<float>();
        private int _currentDirectionIndex = 0;

        public override void Init()
        {
            base.Init();
            
            WheelJoint2D wheelJoint2D = GetComponent<WheelJoint2D>();
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            
            if (wheelJoint2D)
            {
                JointMotor2D motor = wheelJoint2D.motor;
                motor.motorSpeed = ats[EWAt.Speed].Value * 360;
                wheelJoint2D.motor = motor;
                ats[EWAt.Speed].RegisterWithInitValue(value =>
                {
                    JointMotor2D motor = wheelJoint2D.motor;
                    motor.motorSpeed = value * 360;
                    wheelJoint2D.motor = motor;
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
            else if (rb2D)
            {
                rb2D.angularVelocity = ats[EWAt.Speed].Value * 360;
                ats[EWAt.Speed].RegisterWithInitValue(value => rb2D.angularVelocity = value * 360)
                    .UnRegisterWhenGameObjectDestroyed(this);
            }
            
            ats[EWAt.Area].RegisterWithInitValue(value =>
            {
                transform.localScale = new Vector3(value, value);
            }).UnRegisterWhenGameObjectDestroyed(this);

            ats[EWAt.Amount].RegisterWithInitValue(value =>
            {
                GenerateShootingDirections();
            });
        }

        #if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(Orbiter))]
        public class RepeatTileControllerEditor : UnityEditor.Editor
        {
        	public override void OnInspectorGUI()
        	{
        		base.OnInspectorGUI();
        
        		if (GUILayout.Button("Generate Shooting Directions"))
        		{
        			var controller = target as Orbiter;
        			controller.GenerateShootingDirections();
        		}
        	}
        }
        #endif
        
        public void GenerateShootingDirections()
        {
            spawningDirections.Clear();
            int count = ats[EWAt.Amount];
            float angle = 360f / count;
            for (int i = 0; i < count; i++)
            {
                spawningDirections.Add(angle * i);
            }
            LogKit.I("Generated ShootingDirections: " + spawningDirections);
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
            if (spawningDirections.Count > 0)
            {
                _currentDirectionIndex += 1;
                _currentDirectionIndex %= spawningDirections.Count > 0 ? spawningDirections.Count : 1;
                float angle = Util.GetAngleFromVector(direction);
                angle += spawningDirections[_currentDirectionIndex];
                direction = Util.GetVectorFromAngle(angle);
            }
            return direction * attackRange;
        }
    }
}