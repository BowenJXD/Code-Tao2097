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
        private Rigidbody2D rb;
        private SpriteRenderer sp;
        
        [SerializeField] public BindableStat SPD = new BindableStat(1);
        public BindableStat acceleration = new BindableStat(0);
        [ReadOnly] public BindableProperty<Vector2> movementDirection = new BindableProperty<Vector2>(Vector2.zero);
        [HideInInspector] public BindableProperty<Vector2> lastNonZeroDirection = new BindableProperty<Vector2>(Vector2.up);
        public bool trackMovement = false;
        [HideInInspector] public float distanceMoved; 
        List<(float, Action)> distanceEvents = new List<(float, Action)>();
        private bool flipped;

        private void Start()
        {
            rb = Unit.GetComponent<Rigidbody2D>();
            sp = this.GetComponentFromUnit<SpriteRenderer>();
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
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            if (trackMovement)
            {
                distanceMoved += rb.velocity.magnitude * dt;
                for (int i = distanceEvents.Count - 1; i >= 0; i--)
                {
                    var distanceEvent = distanceEvents[i];
                    if (distanceMoved > distanceEvent.Item1)
                    {
                        distanceEvent.Item2?.Invoke();
                        distanceEvents.RemoveAt(i);
                    }
                }
            }
            if (acceleration.Value != 0)
            {
                float oldSpeed = SPD.Value;
                SPD.AddModifier(acceleration * dt, EModifierType.Additive, "acceleration",
                    RepetitionBehavior.AddStack);
                if (oldSpeed > 0 != SPD > 0 && sp)
                {
                    sp.flipX = !sp.flipX;
                    sp.flipY = !sp.flipY;
                    flipped = true;
                }
            }
        }

        public void RegisterDistanceEvent(float distance, Action action)
        {
            distanceEvents.Add((distanceMoved + distance, action));
        }

        private void OnDisable()
        {
            SPD.Reset();
            acceleration.Reset();
            movementDirection = new BindableProperty<Vector2>(Vector2.zero);
            lastNonZeroDirection = new BindableProperty<Vector2>(Vector2.up);
            if (flipped)
            {
                sp.flipX = !sp.flipX;
                sp.flipY = !sp.flipY;
                flipped = false;
            }
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

