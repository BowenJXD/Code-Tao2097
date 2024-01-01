using System;
using System.Collections.Generic;
using CodeTao;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class BuffOwner : Container<Buff>
    {
        [HideInInspector] public AttributeController attributeController;
        
        public void Start()
        {
            attributeController = ComponentUtil.GetComponentFromUnit<AttributeController>(this);
        }
    }
}