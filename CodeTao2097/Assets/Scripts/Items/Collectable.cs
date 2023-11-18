using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class Collectable : ViewController
    {
        public List<ETag> collectingTags = new List<ETag>();
        
        [HideInInspector] public Collider2D collider2D;
        
        protected virtual void Start()
        {
            collider2D.OnTriggerEnter2DEvent(col =>
            {
                if (ValidateCollect(col))
                {
                    Collect(col);
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public virtual bool ValidateCollect(Collider2D col)
        {
            UnitController unitController = ComponentUtil.GetComponentInAncestors<UnitController>(col);
            if (Util.IsTagIncluded(unitController.tag, collectingTags))
            {
                return true;
            }

            return false;
        }
        
        public virtual void Collect(Collider2D collector = null)
        {
            Destroy(gameObject);
        }
    }
}