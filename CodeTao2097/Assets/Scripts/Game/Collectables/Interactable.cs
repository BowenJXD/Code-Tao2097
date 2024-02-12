using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 可以被玩家交互的单位，包括NPC、道具等。
    /// </summary>
    public class Interactable : UnitController
    {
        public List<EntityTag> interactingTags = new List<EntityTag>();
        
        [HideInInspector] public Collider2D interactableCol;

        public override void SetUp()
        {
            base.SetUp();
            if (!interactableCol){
                interactableCol = this.GetCollider((int)ELayer.Interactable);
            }
        }

        public override void Init()
        {
            base.Init();
            
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