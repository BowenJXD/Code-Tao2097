using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    public class WeaponSelector : WeaponComponent
    {
        public virtual List<Vector3> GetGlobalPositions()
        {
            return new List<Vector3>{transform.position};
        }
    }
}