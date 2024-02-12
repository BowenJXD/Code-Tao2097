using System.Collections.Generic;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    ///  单位的选择范围
    /// </summary>
    public enum SelectRange
    {
        Instance,
        Prefab,
        Type,
        EntityTag,
        All,
    }
    
    /// <summary>
    ///  选择单位行为，根据选择范围选择单位。
    /// </summary>
    public class SelectUnitBehaviour : ConditionBehaviour
    {
        public SelectRange selectRange;
        public UnitController baseUnit;

        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            if (!baseUnit) baseUnit = sequence.Get<UnitController>(BBKey.OWNER);
            if (!baseUnit) baseUnit = this.GetComponentInDescendants<UnitController>(true);
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            if (baseUnit)
            {
                List<UnitController> units = new();
                switch (selectRange)
                {
                    case SelectRange.Instance:
                        units.Add(baseUnit);
                        break;
                    case SelectRange.Prefab:
                        units.AddRange(UnitManager.Instance.GetAll(baseUnit));
                        break;
                    case SelectRange.Type:
                        units.AddRange(UnitManager.Instance.FindAll(unit => unit.GetType() == baseUnit.GetType()));
                        break;
                    case SelectRange.EntityTag:
                        units.AddRange(UnitManager.Instance.FindAll(unit => unit.CompareTag(baseUnit.tag)));
                        break;
                    case SelectRange.All:
                        units.AddRange(UnitManager.Instance.FindAll());
                        break;
                }
                units = Process(units);
                sequence.Set(BBKey.TARGETS, units);
                sequence.Set(BBKey.TARGET_COUNT, units.Count);
            }
        }
        
        protected virtual List<UnitController> Process(List<UnitController> units)
        {
            return units;
        }
    }
}