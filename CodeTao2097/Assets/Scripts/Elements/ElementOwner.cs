using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class ElementOwner : ViewController, IContainer<Element>
    {
        public List<IContent<Element>> Contents { get; set; }
        public Action<IContent<Element>> AddAfter { get; set; }
        public Action<IContent<Element>> RemoveAfter { get; set; }

        public void Start()
        {
            
        }

        public void AddElement(Element element, Damage damage = null)
        {
            if (element.Type == ElementType.None || element.Gauge <= 0)
                return;
            IContainer<Element> container = this;
            if (container.AddContent(element))
            {
                ProcessAddedElement(damage);
            }
        }

        public void ProcessAddedElement(Damage damage)
        {
            if (Contents.Count > 1)
            {
                Element elementA = (Element) Contents[0];
                Element elementB = (Element) Contents[1];
                
                Reaction reaction = ReactionManager.Instance.GetReaction(elementA.Type, elementB.Type);
                reaction.React(this, damage);
            }
        }
    }
}
