using QFramework;

namespace CodeTao
{
    public class IntervalTC : TriggerCondition
    {
        public override void Init()
        {
            base.Init();
            triggerCooldown.RegisterWithInitValue(value =>
            {
                buffToTrigger.buffLoop.LoopInterval = value;
            }).UnRegisterWhenGameObjectDestroyed(buffToTrigger);
        }
    }
}