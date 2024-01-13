using QFramework;

namespace CodeTao
{
    /// <summary>
    /// 爆炸单位的管理器，会覆盖爆炸单位的DMG和击退系数。
    /// </summary>
    public class ExplosionGenerator : UnitGenerator<Explosion, ExplosionGenerator>
    {
        public Damager defaultDamager;

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            defaultDamager = this.GetComponentInDescendants<Damager>();
        }

        public Damager ModDamager(Damager damager)
        {
            if (damager == null) damager = defaultDamager;
            else {
                damager.DMG = defaultDamager.DMG;
                damager.knockBackFactor = defaultDamager.knockBackFactor;
            }
            return damager;
        }
    }
}