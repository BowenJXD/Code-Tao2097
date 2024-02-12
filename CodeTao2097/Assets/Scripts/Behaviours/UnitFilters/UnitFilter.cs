using System.Collections.Generic;

namespace CodeTao
{
    /// <summary>
    ///  单位过滤器基类
    /// </summary>
    public abstract class UnitFilter : BehaviourNode
    {
        protected override void OnExecute()
        {
            base.OnExecute();
            if (sequence.TryGet(BBKey.TARGETS, out List<UnitController> units))
            {
                units = Filter(units);
                sequence.Set(BBKey.TARGETS, units);
                sequence.Set(BBKey.TARGET_COUNT, units.Count);
            }
        }

        protected abstract List<UnitController> Filter(List<UnitController> units);
    }
}