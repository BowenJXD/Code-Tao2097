using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 武器选择器，用于生成攻击的位置信息。
    /// </summary>
    public class WeaponSelector : WeaponComponent
    {
        public virtual List<Vector3> GetGlobalPositions()
        {
            return new List<Vector3>{transform.position};
        }
    }
}