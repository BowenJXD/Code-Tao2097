using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;


namespace CodeTao 
{
    [Serializable]
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
    
    [Serializable]
    public class Element : Content<Element>
    {
        [SerializeField] private ElementType _type;
        public ElementType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        
        [SerializeField] private float _gauge;
        public float Gauge
        {
            get { return _gauge; }
            set { _gauge = value; }
        }

        public Color GetColor()
        {
            switch (Type)
            {
                case ElementType.Metal:
                    return Color.yellow;
                case ElementType.Wood:
                    return Color.green;
                case ElementType.Water:
                    return Color.blue;
                case ElementType.Fire:
                    return Color.red;
                case ElementType.Earth:
                    return new Color(165,42,42);
                default:
                    return Color.white;
            }
        }
    }
}

