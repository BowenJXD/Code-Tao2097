using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public partial class Stinky : Weapon
    {
        protected override void Start()
        {
            base.Start();
            IContainer<Item> itemContainer = Player.Instance.Inventory;
            
            // put self into player's inventory
            ActionKit.DelayFrame(1, () =>
            {
                if (!this) return;
                itemContainer.AddContent(this);
            }).Start(this);
            damager = Damager;
        }

        public override void Fire()
        {
            List<Collider2D> colliders = Physics2D.OverlapCircleAll(transform.position, attackRange).ToList();
			
            foreach (var col in colliders)
            {
                UnitController unitController = ComponentUtil.GetComponentInAncestors<UnitController>(col);
                Defencer target = ComponentUtil.GetComponentInDescendants<Defencer>(unitController);
                if (Util.IsTagIncluded(unitController.tag, Damager.damagingTags) && target)
                {
                    Attack(target);
                }
            }
        }
    }
}