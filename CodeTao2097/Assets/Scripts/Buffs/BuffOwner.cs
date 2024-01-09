using System;
using System.Collections.Generic;
using CodeTao;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// Container for buffs.
    /// </summary>
    public class BuffOwner : Container<Buff>
    {
        [HideInInspector] public AttributeController attributeController;
        
        public void Start()
        {
            attributeController = ComponentUtil.GetComponentFromUnit<AttributeController>(this);
        }
    }
}