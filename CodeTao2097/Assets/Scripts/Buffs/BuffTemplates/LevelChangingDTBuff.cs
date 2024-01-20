namespace CodeTao
{
    /// <summary>
    /// 基于自身等级增幅伤害，然后修改自身等级的<see cref="DamageTagBuff"/>
    /// </summary>
    public class LevelChangingDTBuff : DamageTagBuff
    {
        public float damageIncrement = 0.1f;
        public int lvlIncrement = 1;
        protected override Damage ProcessDamage(Damage damage)
        {
            damage.SetDamageSection(DamageSection.DamageIncrement, name, damageIncrement * LVL, RepetitionBehavior.Overwrite);
            AlterLVL(lvlIncrement);
            return damage;
        }
    }
}