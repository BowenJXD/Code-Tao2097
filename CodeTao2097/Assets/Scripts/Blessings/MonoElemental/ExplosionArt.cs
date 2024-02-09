using QFramework;

namespace CodeTao
{
    public class ExplosionArt : BuffingBlessing
    {
        public LoopTask loopTask;
        public float interval = 1;
        public bool doApply;

        public override void Init()
        {
            base.Init();
            loopTask = new LoopTask(this, interval, () => doApply = true);
            loopTask.Start();
        }

        public override bool CheckCondition(Damage damage)
        {
            bool result = doApply && base.CheckCondition(damage);
            doApply = false;
            return result;
        }
    }
}