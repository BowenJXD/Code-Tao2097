using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public class Collectable : UnitController
    {
        public List<ETag> collectingTags = new List<ETag>();
        
        [HideInInspector] public Collider2D col2D;
        
        protected virtual void Start()
        {
            col2D.OnTriggerEnter2DEvent(col =>
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
            if (unitController)
            {
                if (Util.IsTagIncluded(unitController.tag, collectingTags))
                {
                    return true;
                }
            }

            return false;
        }
        
        public virtual void Collect(Collider2D collector = null)
        {
            Destroy(gameObject);
        }
    }
}