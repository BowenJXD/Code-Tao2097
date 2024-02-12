using System.Collections.Generic;

namespace CodeTao
{
    public class OnUnitInitCondition : SelectUnitBehaviour
    {
        protected override List<UnitController> Process(List<UnitController> units)
        {
            base.OnExecute();
            UnNext();
            switch (selectRange)
            {
                case SelectRange.Instance:
                    baseUnit.onDeinit += Next;
                    break;
                case SelectRange.Prefab:
                    UnitManager.Instance.AddOnGetAction(baseUnit, OnGet);
                    break;
                case SelectRange.Type:
                    UnitManager.Instance.AddOnGetAction(baseUnit.GetType(), OnGet);
                    break;
            }
            return units;
        }
        
        protected virtual void OnGet(UnitController unit)
        {
            Next();
        }
    }
}