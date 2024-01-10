using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 碰撞器管理器，用于管理碰撞器与单位的关系，并提供更快捷的检索方式
    /// </summary>
    public class ColliderManager : MonoSingleton<ColliderManager>
    {
        public Dictionary<Collider2D, UnitController> colUnitDict = new Dictionary<Collider2D, UnitController>();
        public Dictionary<UnitController, List<Collider2D>> unitColDict = new Dictionary<UnitController, List<Collider2D>>();

        public void Register(UnitController unit, Collider2D col)
        {
            colUnitDict.Add(col, unit);
            if (!unitColDict.ContainsKey(unit))
            {
                unitColDict.Add(unit, new List<Collider2D>());
            }
            unitColDict[unit].Add(col);
        }
        
        public bool CheckRegistered(UnitController unit)
        {
            return unitColDict.ContainsKey(unit);
        }
        
        public Collider2D GetCollider(UnitController unit, int layer = 0)
        {
            if (unitColDict.ContainsKey(unit) && unitColDict[unit].Count > 0)
            {
                if (layer == 0)
                {
                    return unitColDict[unit].FirstOrDefault();
                }
                foreach (var col in unitColDict[unit])
                {
                    if (col.gameObject.layer == layer)
                    {
                        return col;
                    }
                }
            }

            return null;
        }
    }
    
    
}