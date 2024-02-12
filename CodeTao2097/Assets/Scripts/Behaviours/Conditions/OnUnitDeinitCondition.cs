using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    ///  单位销毁条件，当指定的单位类型销毁时，执行下一个行为。
    /// </summary>
    public class OnUnitDeinitCondition : SelectUnitBehaviour
    {
        protected override List<UnitController> Process(List<UnitController> units)
        {
            UnNext();
            switch (selectRange)
            {
                case SelectRange.Instance:
                    baseUnit.onDeinit += Next;
                    break;
                case SelectRange.Prefab:
                    UnitManager.Instance.AddOnReleaseAction(baseUnit, OnRelease);
                    break;
                case SelectRange.Type:
                    UnitManager.Instance.AddOnReleaseAction(baseUnit.GetType(), OnRelease);
                    break;
            }
            return base.Process(units);
        }
        
        protected virtual void OnRelease(UnitController unit)
        {
            var position = unit.transform.position;
            sequence.Set(BBKey.GLOBAL_POS, new List<Vector3>{position});
            sequence.Set(BBKey.LOCAL_POS, new List<Vector3>{position - transform.position});
            Next();
        }
    }
}