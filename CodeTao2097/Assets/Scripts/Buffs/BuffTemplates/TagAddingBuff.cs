namespace CodeTao
{
    /// <summary>
    /// Adds a tag to the unit.
    /// </summary>
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