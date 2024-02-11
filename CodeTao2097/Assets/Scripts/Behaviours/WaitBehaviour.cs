using System.Collections;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    ///  等待行为，等待一段时间后执行下一个行为。
    /// </summary>
    public class WaitBehaviour : BehaviourNode
    {
        public float time;
        
        protected override IEnumerator Executing()
        {
            yield return new WaitForSeconds(time);
        }
    }
}