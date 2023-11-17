using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class MoveController : ViewController
    {
        [SerializeField] public BindableStat SPD = new BindableStat(1);
        
        private Vector2 _moveDirection = Vector2.zero;
        
        public Vector2 MovementDirection { 
            get { return _moveDirection; }
            set { _moveDirection = value; } 
        }
    }
}

