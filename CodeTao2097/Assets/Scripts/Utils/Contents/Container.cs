using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public abstract class Container<T> : ViewController where T : Content<T>
    {
        [HideInInspector] public List<Content<T>> Contents;

        public Action<Content<T>> AddAfter;

        public Action<Content<T>> RemoveAfter;
        
        /// <summary>
        /// Should only be invoked from Content
        /// </summary>
        /// <param name="newContent"></param>
        /// <param name="repetitionBehavior"></param>
        /// <returns></returns>
        public virtual bool AddContent(Content<T> newContent, ERepetitionBehavior repetitionBehavior = ERepetitionBehavior.Return)
        {
            bool result = false;
            
            if (Contents == null)
            {
                Contents = new List<Content<T>>();
            }

            var matches = Contents.FindAll(delegate(Content<T> content)
            {
                return content.Equals(newContent);
            });
            
            if (matches.Count > 0)
            {
                switch (repetitionBehavior)
                {
                    case ERepetitionBehavior.Return:
                        break;
                    case ERepetitionBehavior.Overwrite:
                        matches.ForEach(delegate(Content<T> content)
                        {
                            Contents.Remove(content);
                        });
                        result = true;
                        break;
                    case ERepetitionBehavior.AddStack:
                        matches[0].Stack(newContent);
                        break;
                    case ERepetitionBehavior.NewStack:
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
                Contents.Add(newContent);
                AddAfter?.Invoke(newContent);
                ProcessAddedContent(newContent);
            }
            
            return result;
        }

        public virtual void ProcessAddedContent(Content<T> content)
        {
            content.Parent(transform);
        }
        
        /// <summary>
        /// Should only be invoked from Content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public virtual bool RemoveContent(Content<T> content)
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

        public virtual void ProcessRemovedContent(Content<T> content) { }
    }
}