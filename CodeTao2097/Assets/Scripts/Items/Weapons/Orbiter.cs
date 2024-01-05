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
        }

        /*#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(Orbiter))]
        public class OrbiterEditor : UnityEditor.Editor
        {
        	public override void OnInspectorGUI()
        	{
        		base.OnInspectorGUI();
        
        		if (GUILayout.Button("Generate Spawning Directions"))
        		{
        			var controller = target as Orbiter;
        			controller.GenerateShootingDirections();
        		}
        	}
        }
        #endif
        
        public void GenerateShootingDirections()
        {
            spawningDirections = Util.GenerateAngles(ats[EWAt.Amount]);
            LogKit.I("Generated SpawningDirections: " + spawningDirections);
        }*/
        
        public override Projectile SpawnUnit(Vector2 localPos)
        {
            Projectile unit = base.SpawnUnit(localPos);
            unit.lifeTime.Value = ats[EWAt.Duration].Value;
            unit.Parent(transform)
                .Rotation(Quaternion.Euler(0, 0, Util.GetAngleFromVector(localPos.normalized)))
                .SetWeapon(this)
                .SetMovingDirection(Vector2.zero);
            return unit;
        }
        
        public override Vector2 GetLocalSpawnPoint(Vector2 basePoint, int spawnIndex)
        {
            float angle = Util.GetAngleFromVector(basePoint);
            if (spawningDirections.Count > 0)
            {
                _currentDirectionIndex += 1;
                _currentDirectionIndex %= spawningDirections.Count > 0 ? spawningDirections.Count : 1;
                angle += spawningDirections[_currentDirectionIndex];
            }
            else
            {
                float amount = ats[EWAt.Amount].Value;
                angle = spawnIndex * 360 / amount;
            }
            
            return Util.GetVectorFromAngle(angle) * AttackRange;
        }
    }
}