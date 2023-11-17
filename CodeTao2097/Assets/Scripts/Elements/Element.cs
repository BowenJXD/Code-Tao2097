using System;
using System.Collections;
using System.Collections.Generic;
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
    public class Element : IContent<Element>
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

        public IContainer<Element> Container { get; set; }
    }
}

