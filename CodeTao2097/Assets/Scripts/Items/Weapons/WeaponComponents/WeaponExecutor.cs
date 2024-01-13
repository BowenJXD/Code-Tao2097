using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    public class WeaponExecutor : WeaponComponent
    {
        public bool next = false;
        
        public virtual void Execute(List<Vector3> globalPositions)
        {
            next = false;
        }

        protected virtual void Next()
        {
            next = true;
        }
    }
}