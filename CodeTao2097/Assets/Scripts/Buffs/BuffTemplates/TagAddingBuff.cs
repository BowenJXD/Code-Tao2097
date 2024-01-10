using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// Adds a tag to the unit.
    /// </summary>
    public class TagAddingBuff : Buff
    {
        public Tag tagToAdd;
        protected TagOwner tagOwner;
        
        public override void Init()
        {
            base.Init();
            tagOwner = Container.GetComp<TagOwner>();
            if (tagOwner)
            {
                tagOwner.AddTag(tagToAdd);
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            tagOwner.RemoveTag(tagToAdd);
        }
    }
}