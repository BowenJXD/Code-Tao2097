using QFramework;

namespace CodeTao
{
    public class ExplosiveBuff : Buff
    {
        private Defencer defencer;
        
        public override void OnAdd()
        {
            base.OnAdd();
            defencer = Container.GetComp<Defencer>();
            if (defencer) defencer.TakeDamageAfter += TakeDamageAfter;
        }

        void TakeDamageAfter(Damage damage)
        {
            if (damage.Source && damage.GetDamageSection(DamageSection.CRIT) != 1)
            {
                SpawnExplosion();
            }
        }
        
        void SpawnExplosion()
        {
            Explosion obj = ExplosionGenerator.Instance.Get();
            if (obj)
            {
                obj.Position(defencer.transform.position);
                obj.Init();
            }
        }
    }
}