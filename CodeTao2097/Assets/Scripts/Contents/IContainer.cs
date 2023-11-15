using System.Collections.Generic;

namespace CodeTao
{
    public interface IContainer<T> where T : IContent<T>
    {
        public List<IContent<T>> Contents { get; set; }
        
        
        /// <summary>
        /// Should only be invoked from Content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="repetitionBehavior"></param>
        /// <returns></returns>
        public bool AddContent(IContent<T> content, ERepetitionBehavior repetitionBehavior = ERepetitionBehavior.Return)
        {
            bool result = false;
            if (Contents == null)
            {
                Contents = new List<IContent<T>>();
            }
            
            if (Contents.Contains(content))
            {
                switch (repetitionBehavior)
                {
                    case ERepetitionBehavior.Return:
                        result = false;
                        break;
                    case ERepetitionBehavior.Overwrite:
                        Contents.Remove(content);
                        Contents.Add(content);
                        result = true;
                        break;
                    case ERepetitionBehavior.AddStack:
                        Contents.Add(content);
                        result = true;
                        break;
                }
            }
            else
            {
                Contents.Add(content);
                result = true;
                return true;
            }

            return result;
        }
        
        /// <summary>
        /// Should only be invoked from Content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool RemoveContent(IContent<T> content)
        {
            bool result = false;
            if (Contents == null)
            {
                return result;
            }
            
            result = Contents.Remove(content);
            return result;
        }
    }
}