using System;
using System.Collections.Generic;
using QFramework;

namespace CodeTao
{
    public class BlessingOwner : ViewController, IContainer<Blessing>
    {
        public List<IContent<Blessing>> Contents { get; set; }
        public Action<IContent<Blessing>> AddAfter { get; set; }
        public Action<IContent<Blessing>> RemoveAfter { get; set; }

        public void ProcessAddedContent(IContent<Blessing> content)
        {
            
        }
    }
}