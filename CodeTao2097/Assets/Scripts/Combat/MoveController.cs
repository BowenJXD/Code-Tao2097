using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public partial class MoveController : ViewController
    {
        [SerializeField] public BindableStat SPD = new BindableStat(1);

        public BindableProperty<Vector2> MovementDirection = new BindableProperty<Vector2>(Vector2.zero);
    }
}

