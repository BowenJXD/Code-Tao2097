using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 负责管理移动的控制器，包括移动速度，移动方向，以及最后一次移动的方向。
    /// </summary>
    public partial class MoveController : UnitComponent, IAAtReceiver, IWAtReceiver
    {
        [SerializeField] public BindableStat SPD = new BindableStat(1);
        public BindableStat acceleration = new BindableStat(0);
        private Rigidbody2D rb;

        public BindableProperty<Vector2> movementDirection = new BindableProperty<Vector2>(Vector2.zero);
        
        [ReadOnly] public BindableProperty<Vector2> lastNonZeroDirection = new BindableProperty<Vector2>(Vector2.up);

        private void Start()
        {
            rb = Unit.GetComponent<Rigidbody2D>();
            if (!rb) return;
            SPD.Init();
            SPD.RegisterWithInitValue(value =>
            {
                rb.velocity = movementDirection.Value * value;
            }).UnRegisterWhenGameObjectDestroyed(this);
            movementDirection.RegisterWithInitValue(value =>
            {
                rb.velocity = value * SPD.Value;
                if (value != Vector2.zero)
                {
                    lastNonZeroDirection.Value = value;
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
            acceleration.Init();
            if (acceleration.Value != 0)
            {
                ActionKit.Coroutine(Accelerate).Start(this);
            }
        }
        
        IEnumerator Accelerate()
        {
            while (gameObject.activeSelf)
            {
                SPD.AddModifier(acceleration * Time.deltaTime, EModifierType.Additive, "acceleration",
                    RepetitionBehavior.AddStack);
                yield return null;
            }
        }

        private void OnDisable()
        {
            SPD.Reset();
            acceleration.Reset();
            movementDirection = new BindableProperty<Vector2>(Vector2.zero);
            lastNonZeroDirection = new BindableProperty<Vector2>(Vector2.up);
        }

        public void Receive(IAAtSource source)
        {
            SPD.InheritStat(source.GetAAt(EAAt.SPD));
        }

        public void Receive(IWAtSource source)
        {
            SPD.InheritStat(source.GetWAt(EWAt.Speed));
        }
    }
}

