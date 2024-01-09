using System;
using QFramework;

namespace CodeTao
{
    public class Blessing : Item
    {
        public override void OnRemove()
        {
            base.OnRemove();
            enabled = false;
        }
    }
}