using System.Collections;

namespace CodeTao
{
    /// <summary>
    ///  开始行为，开始执行行为数列。无视条件。
    /// </summary>
    public class StartSequenceBehaviour : BehaviourNode
    {
        public bool ignoreOtherConditions = false;
        
        protected override void OnExecute()
        {
            base.OnExecute();
            if (ignoreOtherConditions)
            { 
                sequence.Continue();
            }
            else
            {
                sequence.SetIndexConditionMet(true);
            }
        }
    }
}