using System.Collections.Generic;

namespace CodeTao
{
    /// <summary>
    ///  有指定buff的单位过滤器
    /// </summary>
    public class BuffUnitFilter : UnitFilter
    {
        public List<ElementType> buffElements;

        protected override List<UnitController> Filter(List<UnitController> units)
        {
            List<UnitController> result = new List<UnitController>();
            foreach (UnitController unit in units)
            {
                if (unit.TryGetComp(out BuffOwner buffOwner))
                {
                    if (buffOwner.FindAll(buff => buffElements.Contains(buff.elementType)).Count > 0)
                    {
                        result.Add(unit);
                    }
                }
            }

            return result;
        }
    }
}