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
    
    public class Element : IContent<Element>
    {
        private ElementType _type;
        public ElementType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        
        private float mAura;
        public float Gauge
        {
            get { return mAura; }
            set { mAura = value; }
        }

        public IContainer<Element> Container { get; set; }

        public bool AddToContainer(IContainer<Element> container)
        {
            throw new System.NotImplementedException();
        }
    }
}

