using System.Collections;
using GraphProcessor;
using Unity.Collections;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 行为节点，受行为数列控制。拥有一个IEnumerator，用于执行行为。
    /// </summary>
    public abstract class BehaviourNode : MonoBehaviour
    {
        protected BehaviourSequence sequence;
        [ReadOnly][ShowInInspector] protected bool next = true;

        public virtual void Init(BehaviourSequence newSequence)
        {
            sequence = newSequence;
        }
        
        public virtual IEnumerator Execute()
        {
            OnExecute();
            yield return Executing();
            yield return new WaitUntil(() => next);
        }
        
        protected virtual void OnExecute()
        {
            
        }

        protected virtual IEnumerator Executing()
        {
            yield return null;
        }
        
        public void UnNext()
        {
            next = false;
        }
        
        public void Next()
        {
            next = true;
        }
    }
}