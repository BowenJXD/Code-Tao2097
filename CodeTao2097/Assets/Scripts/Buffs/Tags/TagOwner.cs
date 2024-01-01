using System.Collections.Generic;
using QFramework;

namespace CodeTao
{
    public class TagOwner : ViewController
    {
        public List<Tag> tags = new List<Tag>();
        
        public bool HasTag(Tag newTag)
        {
            return tags.Contains(newTag);
        }
        
        public bool AddTag(Tag newTag)
        {
            if (tags.Contains(newTag))
            {
                return false;
            }
            
            tags.Add(newTag);
            return true;
        }
        
        public bool RemoveTag(Tag newTag)
        {
            if (!tags.Contains(newTag))
            {
                return false;
            }
            
            tags.Remove(newTag);
            return true;
        }
    }
}