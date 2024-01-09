using System;
using QFramework;

namespace CodeTao
{
    /// <summary>
    /// 祝福母类，用于实现祝福的基本功能。祝福无法升级。通过开宝箱获得。
    /// blessing is a special item that can't be upgraded. It can only be obtained by opening a chest.
    /// </summary>
    public abstract class Blessing : Item
    {
        public override void OnRemove()
        {
            base.OnRemove();
            enabled = false;
        }
    }
}