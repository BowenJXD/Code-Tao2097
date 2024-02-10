using QFramework;

namespace CodeTao
{
    /// <summary>
    /// 共振涟漪（金水）
    /// 令回流物品在触碰到参与共振的个体时，触发该个体的共振效果。
    /// 需要backflow，通过backflow的damager触发。
    /// Resonating Flow (metal & water)
    /// Make the backflow item trigger the resonance effect of the individual when it touches the individual participating in the resonance.
    /// </summary>
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
                TagOwner tagOwner = damage.Target.GetComp<TagOwner>();
                if (tagOwner && tagOwner.HasTag(Tag.Resonating))
                {
                    ResonanceWaveGenerator.Instance.Get().Position(tagOwner.transform.position).Init();
                }
            };
        }
        
        public override int GetWeight()
        {
            if (!Container) return 0;
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