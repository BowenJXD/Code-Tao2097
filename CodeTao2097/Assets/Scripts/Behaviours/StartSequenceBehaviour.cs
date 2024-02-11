using System.Collections;

namespace CodeTao
{
    /// <summary>
    ///  开始行为，开始执行行为数列。无视条件。
    /// </summary>
    public class StartSequenceBehaviour : BehaviourNode
    {
        protected override void OnExecute()
        {
            base.OnExecute();
            sequence.StartSequence();
        }
    }
}