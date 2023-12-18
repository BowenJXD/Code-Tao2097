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
            UnitController unitController = ComponentUtil.GetComponentInAncestors<UnitController>(this);
            unitController.onDeinit += ClearBuff;
        }

        public void ClearBuff()
        {
            for (int i = Contents.Count - 1; i >= 0; i--)
            {
                Contents[i].RemoveFromContainer(this);
            }
        }
    }
}