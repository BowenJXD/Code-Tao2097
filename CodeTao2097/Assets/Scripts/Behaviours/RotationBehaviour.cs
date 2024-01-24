using System;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class RotationBehaviour : MonoBehaviour, IWAtReceiver
    {
        public BindableStat angularSpeed = new BindableStat(-180f);
        private WheelJoint2D wheelJoint2D;

        private void OnEnable()
        {
            angularSpeed.Init();
            
            wheelJoint2D = GetComponent<WheelJoint2D>();
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            if (wheelJoint2D)
            {
                JointMotor2D motor = wheelJoint2D.motor;
                motor.motorSpeed = angularSpeed.Value;
                wheelJoint2D.motor = motor;
                angularSpeed.RegisterWithInitValue(value =>
                {
                    JointMotor2D motor = wheelJoint2D.motor;
                    motor.motorSpeed = value;
                    wheelJoint2D.motor = motor;
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
        }

        private void Update()
        {
            if (!wheelJoint2D) transform.Rotate(Vector3.forward, angularSpeed * Time.deltaTime);
        }

        private void OnDisable()
        {
            angularSpeed.Reset();
        }

        public void Receive(IWAtSource source)
        {
            angularSpeed.InheritStat(source.GetWAt(EWAt.Speed));
        }
    }
}