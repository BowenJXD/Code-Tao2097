using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public class Interactable : UnitController
    {
        public List<EntityType> interactingTags = new List<EntityType>();
        
        [HideInInspector] public Collider2D interactableCol;

        public override void OnSceneLoaded()
        {
            base.OnSceneLoaded();
            if (!interactableCol){
                interactableCol = this.GetComponentInDescendants<Collider2D>(true, (int)ELayer.Interactable);
            }
        }

        public override void PreInit()
        {
            base.PreInit();
            
            interactableCol.OnTriggerEnter2DEvent(col =>
            {
                if (ValidateCollision(col))
                {
                    Interact(col);
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public virtual bool ValidateCollision(Collider2D col)
        {
            UnitController unitController = col.GetComponentInAncestors<UnitController>();
            if (unitController)
            {
                if (Util.IsTagIncluded(unitController.tag, interactingTags))
                {
                    return true;
                }
            }

            return false;
        }
        
        public virtual void Interact(Collider2D col = null)
        {
            Deinit();
        }
    }
}