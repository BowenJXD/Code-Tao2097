using QFramework;

namespace CodeTao
{
    /// <summary>
    /// 爆炸单位的管理器，会覆盖爆炸单位的DMG和击退系数。
    /// </summary>
    public class ExplosionManager : UnitManager<Explosion, ExplosionManager>
    {
        public BindableStat DMG = new BindableStat(10);
        public BindableStat knockBackFactor = new BindableStat(2);

        public override Explosion Get()
        {
            Explosion explosion = base.Get();
            explosion.damager.DMG = DMG;
            explosion.damager.KnockBackFactor = knockBackFactor;
            return explosion;
        }
    }
}