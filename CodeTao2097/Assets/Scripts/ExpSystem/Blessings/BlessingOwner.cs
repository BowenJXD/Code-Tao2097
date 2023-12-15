using System;
using System.Collections.Generic;
using QFramework;

namespace CodeTao
{
    public class BlessingOwner : Container<Blessing>
    {
        private void Start()
        {
            var blessings = FindObjectsOfType<Blessing>();
            foreach (var blessing in blessings)
            {
                blessing.AddToContainer(this);
            }
        }
    }
}