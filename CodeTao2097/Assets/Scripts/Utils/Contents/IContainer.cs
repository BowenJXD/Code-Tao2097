using System;
using System.Collections.Generic;

namespace CodeTao
{
    public interface IContainer<T> where T : IContent<T>
    {
        public List<IContent<T>> Contents { get; set; }
        
        public Action<IContent<T>> AddAfter { get; set; }
        
        public Action<IContent<T>> RemoveAfter { get; set; }
        
        /// <summary>
        /// Should only be invoked from Content
        /// </summary>
        /// <param name="newContent"></param>
        /// <param name="repetitionBehavior"></param>
        /// <returns></returns>
        public bool AddContent(IContent<T> newContent, ERepetitionBehavior repetitionBehavior = ERepetitionBehavior.Return)
        {
            bool result = false;
            
            if (Contents == null)
            {
                Contents = new List<IContent<T>>();
            }

            var matches = Contents.FindAll(delegate(IContent<T> content)
            {
                return content.Equals(newContent);
            });
            
            if (matches.Count > 0)
            {
                switch (repetitionBehavior)
                {
                    case ERepetitionBehavior.Return:
                        result = false;
                        break;
                    case ERepetitionBehavior.Overwrite:
                        matches.ForEach(delegate(IContent<T> content)
                        {
                            Contents.Remove(content);
                        });
                        Contents.Add(newContent);
                        result = true;
                        break;
                    case ERepetitionBehavior.AddStack:
                        result = matches[0].Stack(newContent);
                        break;
                    case ERepetitionBehavior.NewStack:
                        Contents.Add(newContent);
                        result = true;
                        break;
                }
            }
            else
            {
                Contents.Add(newContent);
                result = true;
            }

            if (result)
            {
                AddAfter?.Invoke(newContent);
                ProcessAddedContent(newContent);
            }
            
            return result;
        }

        public void ProcessAddedContent(IContent<T> content) { }
        
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
            
            if (result)
            {
                RemoveAfter?.Invoke(content);
                ProcessRemovedContent(content);
            }
            
            return result;
        }

        public void ProcessRemovedContent(IContent<T> content) { }
    }
}