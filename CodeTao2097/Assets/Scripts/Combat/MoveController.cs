using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 负责管理移动的控制器，包括移动速度，移动方向，以及最后一次移动的方向。
    /// </summary>
    public partial class MoveController : UnitComponent
    {
        [SerializeField] public BindableStat SPD = new BindableStat(1);

        public BindableProperty<Vector2> MovementDirection = new BindableProperty<Vector2>(Vector2.zero);
        
        public BindableProperty<Vector2> LastNonZeroDirection = new BindableProperty<Vector2>(Vector2.up);

        private void Start()
        {
            MovementDirection.RegisterWithInitValue(value =>
            {
                if (value != Vector2.zero)
                {
                    LastNonZeroDirection.Value = value;
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        private void OnDisable()
        {
            SPD.Reset();
            MovementDirection = new BindableProperty<Vector2>(Vector2.zero);
            LastNonZeroDirection = new BindableProperty<Vector2>(Vector2.up);
        }
    }
}

