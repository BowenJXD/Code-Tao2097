using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class MoveController : ViewController
    {
        public BindableStat SPD = new BindableStat(1);
        
        private Vector2 mMoveDirection = Vector2.zero;
        
        public Vector2 MovementDirection { 
            get { return mMoveDirection; }
            set { mMoveDirection = value; } 
        }
    }
}

