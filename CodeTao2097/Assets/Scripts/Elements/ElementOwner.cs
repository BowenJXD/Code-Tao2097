using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class ElementOwner : ViewController, IContainer<Element>
    {
        public List<IContent<Element>> Contents { get; set; }

        public void Start()
        {
        }
    }
}
