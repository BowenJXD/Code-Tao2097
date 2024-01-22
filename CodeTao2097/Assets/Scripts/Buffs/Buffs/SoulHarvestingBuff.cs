using QFramework;

namespace CodeTao
{
    public class SoulHarvestingBuff : DamageTagBuff
    {
        /// <summary>
        /// 0 - 100
        /// </summary>
        public float triggerChance = 20;

        protected override Damage ProcessDamage(Damage damage)
        {
            if (RandomUtil.RandCrit(triggerChance))
            {
                ExpBall expBall = ExpGenerator.Instance.Get().Position(transform.position);
                expBall.EXPValue.Value = 1;
                expBall.Init();
            }
            return base.ProcessDamage(damage);
        }
    }
}