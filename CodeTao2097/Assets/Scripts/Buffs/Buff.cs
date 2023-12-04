using System;
using CodeTao;
using QFramework;

namespace Buffs
{
    public class Buff : IContent<Buff>
    {
        # region IContent
        public BindableProperty<int> LVL { get; set; }
        public BindableProperty<int> MaxLVL { get; set; }
        public IContainer<Buff> Container { get; set; }
        public Action<IContent<Buff>> AddAfter { get; set; }
        public Action<IContent<Buff>> RemoveAfter { get; set; }
        # endregion

        public LoopTask buffLoop;
    }
}