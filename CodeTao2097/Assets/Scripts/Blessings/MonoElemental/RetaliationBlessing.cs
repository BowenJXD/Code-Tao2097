using QFramework;

namespace CodeTao
{
    public class RetaliationBlessing : Blessing
    {
        Damager damager;
        private Attacker attacker;
        
        public override void Init()
        {
            base.Init();
            damager = this.GetComponentInDescendants<Damager>();
            damager.OnDealDamageFuncs.Add(damage =>
            {
                damage.SetDamageSection(DamageSection.SourceStat, "",
                    Container.GetComp<AttributeController>().GetAAt(EAAt.DEF));
                return damage;
            });
            attacker = Container.GetComp<Attacker>();
            Container.Unit.GetCollider().OnTriggerEnter2DEvent(col =>
            {
                DamageManager.Instance.DamageCol(damager, col, attacker, new [] { AttackerUsage.Crit, AttackerUsage.ElementBonus });
            }).UnRegisterWhenGameObjectDestroyed(this);
        }
    }
}