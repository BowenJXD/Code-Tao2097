using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 执行器，基于选择器给予的位置信息执行效果。
    /// 武器拥有的多个执行器会按照顺序执行，在上一个执行器结束后才会执行下一个。
    /// </summary>
    public class WeaponExecutor : WeaponComponent
    {
        public bool next = false;
        
        public virtual void Execute(List<Vector3> globalPositions)
        {
        }
        
        public virtual IEnumerator ExecuteCoroutine(List<Vector3> globalPositions)
        {
            Execute(globalPositions);
            yield return new WaitUntil(() => next);
        }

        protected virtual void Next()
        {
            next = true;
        }
    }
}