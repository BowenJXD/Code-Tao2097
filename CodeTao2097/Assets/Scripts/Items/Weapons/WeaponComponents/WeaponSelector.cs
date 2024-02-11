using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 武器选择器，用于生成攻击的位置信息。
    /// </summary>
    public class WeaponSelector : WeaponBehaviour
    {
        protected override void OnExecute()
        {
            base.OnExecute();
            SetPositions();
        }

        public virtual void SetPositions()
        {
            int amount = (int)weapon.amount.Value;
            List<Vector3> globals = new List<Vector3>();
            List<Vector3> locals = new List<Vector3>();
            for (int i = 0; i < amount; i++)
            {
                Vector3 local = GetLocalPosition(i);
                locals.Add(local);
                globals.Add(transform.position + local);
            }

            sequence.Set(globalPosKey, globals);
            sequence.Set(localPosKey, locals);
        }
        
        public virtual Vector3 GetLocalPosition(int index)
        {
            return Vector3.zero;
        }
    }
}