namespace CodeTao
{
    public abstract class Reaction
    {
        public ElementType[] RelatedElements = new ElementType[2];

        public abstract void React(ElementOwner owner, Damage damage = null);
    }

    public abstract class InstantReaction
    {
        
    }
    
    public abstract class ContinuousReaction
    {
        
    }
}