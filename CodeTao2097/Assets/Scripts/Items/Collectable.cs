using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class Collectable : ViewController
    {
        public List<ETag> collectingTags = new List<ETag>();
        
        public Collider2D collider2D;
        
        private void Start()
        {
            collider2D.OnTriggerEnter2DEvent(col =>
            {
                if (Util.IsTagIncluded(col.gameObject.tag, collectingTags))
                {
                    Collect();
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public void Collect(GameObject collector = null)
        {
            Destroy(gameObject);
        }
    }
}