using System.Collections.Generic;
using QFramework;

namespace CodeTao
{
    public class ReactionManager : MonoSingleton<ReactionManager>
    {
        public List<Reaction> Reactions;
        
        public void RegisterReaction(Reaction reaction)
        {
            Reactions.Add(reaction);
        }
        
    }
}