using System;
using System.Collections.Generic;
using CodeTao;
using QFramework;

namespace Buffs
{
    public class BuffOwner : Container<Buff>
    {
        public void Start()
        {
            UnitController unitController = ComponentUtil.GetComponentInAncestors<UnitController>(this);
            unitController.onDestroy += ClearBuff;
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