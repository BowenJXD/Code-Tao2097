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
        Earth
    }
    
    public class Element
    {
        public ElementType Type;
        public float Aura; // 附着量
    }
}

