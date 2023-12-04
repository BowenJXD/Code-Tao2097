using System.Collections.Generic;
using QFramework;

namespace CodeTao
{
    public class ReactionManager : MonoSingleton<ReactionManager>
    {
        public Dictionary<(ElementType, ElementType), Reaction> Reactions = new Dictionary<(ElementType, ElementType), Reaction>();
        
        protected void Start()
        {
        }
        
        public void RegisterReaction(Reaction reaction)
        {
            var key = (reaction.RelatedElements[0], reaction.RelatedElements[1]);
            Reactions.Add(key, reaction);
        }
        
        public Reaction GetReaction(ElementType a, ElementType b)
        {
            var key = (a, b);
            if (Reactions.ContainsKey(key))
            {
                return Reactions[key];
            }

            return null;
        }
    }
}