using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 在拥有者身上添加标签的buff
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