using System;
using System.Collections.Generic;
using CodeTao;
using QFramework;

namespace Buffs
{
    public class BuffOwner : ViewController, IContainer<Buff>
    {
        public List<IContent<Buff>> Contents { get; set; }
        public Action<IContent<Buff>> AddAfter { get; set; }
        public Action<IContent<Buff>> RemoveAfter { get; set; }
        
        public void Start()
        {
            UnitController unitController = ComponentUtil.GetComponentInAncestors<UnitController>(this);
            unitController.onDestroy += ClearBuff;
        }

        public void ClearBuff()
        {
            foreach (var buff in Contents)
            {
                buff.RemoveFromContainer(this);
            }
        }
    }
}