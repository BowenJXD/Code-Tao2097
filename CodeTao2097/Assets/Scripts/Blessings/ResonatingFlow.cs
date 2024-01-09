using QFramework;

namespace CodeTao
{
    public class ResonatingFlow : Blessing
    {
        public BackFlow backFlow;
        
        public override void OnAdd()
        {
            base.OnAdd();

            if (!backFlow)
            {
                backFlow = Container.GetContent<BackFlow>();
            }
            backFlow.damager.DealDamageAfter += damage =>
            {
                TagOwner tagOwner = damage.Target.GetComponentFromUnit<TagOwner>();
                if (tagOwner && tagOwner.HasTag(Tag.Resonating))
                {
                    ResonanceWaveManager.Instance.Get().Position(tagOwner.transform.position).Init();
                }
            };
        }
        
        public override int GetWeight()
        {
            backFlow = Container.GetContent<BackFlow>();
            if (backFlow)
            {
                return (int)weight;
            }
            else
            {
                return 0;
            }
        }
    }
}