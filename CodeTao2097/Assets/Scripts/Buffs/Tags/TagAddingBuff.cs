namespace CodeTao
{
    public class TagAddingBuff : Buff
    {
        public Tag tag;
        protected TagOwner tagOwner;
        
        public override void Init()
        {
            base.Init();
            tagOwner = Container.GetComponentFromUnit<TagOwner>();
            if (tagOwner)
            {
                tagOwner.AddTag(tag);
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            tagOwner.RemoveTag(tag);
        }
    }
}