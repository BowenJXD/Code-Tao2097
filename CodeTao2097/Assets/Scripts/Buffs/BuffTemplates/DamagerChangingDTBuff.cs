﻿namespace CodeTao
{
    /// <summary>
    /// 修改<see cref="Damager"/>的<see cref="DamageTagBuff"/>
    /// </summary>
    public class DamagerChangingDTBuff : DamageTagBuff
    {
        public float damageIncrement = 0.1f;
        public EModifierType modifierType = EModifierType.MultiAdd;

        protected override Damage ProcessDamage(Damage damage)
        {
            damage.Median.DMG.AddModifier(damageIncrement * LVL, modifierType, name, RepetitionBehavior.Overwrite);
            return damage;
        }
    }
}