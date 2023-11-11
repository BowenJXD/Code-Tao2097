using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CodeTao 
{
    public enum ElementType
    {
        None,
        Metal,
        Wood,
        Water,
        Fire,
        Earth,
        All
    }
    
    public class Element
    {
        private ElementType mType;
        public ElementType Type
        {
            get { return mType; }
            set { mType = value; }
        }
        
        private float mAura;
        public float Aura
        {
            get { return mAura; }
            set { mAura = value; }
        }

        private float mFadeSpeed;
        public float FadeSpeed
        {
            get { return mFadeSpeed; }
            set { mFadeSpeed = value; }
        }
    }
}

