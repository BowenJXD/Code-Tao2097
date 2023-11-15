namespace CodeTao
{
    public abstract class Reaction
    {
        public ElementType[] RelatedElements;

        public abstract void Trigger(ElementOwner owner, Damage damage);
    }

    public abstract class InstantReaction
    {
        
    }
    
    public abstract class ContinuousReaction
    {
        
    }
}