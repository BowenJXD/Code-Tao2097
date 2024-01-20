namespace CodeTao
{
    /// <summary>
    /// 给予拥有者共鸣标签的<see cref="DamageTagBuff"/>，会在受到共鸣伤害时移除共鸣标签
    /// </summary>
    public class ResonatingBuff : DamageTagBuff
    {
        TagOwner tagOwner;
        
        public override void Init()
        {
            base.Init();
            tagOwner = Container.GetComp<TagOwner>();
            if (tagOwner)
            {
                tagOwner.AddTag(Tag.Resonating);
            }
        }

        protected override Damage ProcessDamage(Damage damage)
        {
            RemoveFromContainer(Container);
            return damage;
        }

        public override void OnRemove()
        {
            base.OnRemove();
            if (tagOwner)
            {
                tagOwner.RemoveTag(Tag.Resonating);
            }
        }
    }
}