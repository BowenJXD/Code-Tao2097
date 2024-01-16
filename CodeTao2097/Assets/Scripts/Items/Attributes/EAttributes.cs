namespace CodeTao
{
    /// <summary>
    /// 武器拥有的属性
    /// </summary>
    public enum EWAt
    {
        Null,
        Damage,
        Cooldown,
        Area,
        Amount,
        Duration,
        Speed,
        Range,
        
        KnockBack = 9,
        EffectHitRate,
    }

    /// <summary>
    /// 奇物能影响的单位属性
    /// </summary>
    public enum EAAt
    {
        Null,
        ATK,
        DEF,
        MaxHP,
        SPD,
        CritRate,
        CritDamage,
        AllElementBON,
        MetalElementBON,
        WoodElementBON,
        WaterElementBON,
        FireElementBON,
        EarthElementBON,
        AllElementRES,
        MetalElementRES,
        WoodElementRES,
        WaterElementRES,
        FireElementRES,
        EarthElementRES,
        EXPBonus,
        Lives,
    }
}