using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class BlessingChest : Interactable
    {
        public override void Interact(Collider2D col = null)
        {
            base.Interact(col);
            if (col)
            {
                UIKit.GetPanel<UIGamePanel>().OpenExpUpgradePanel(new [] { ItemType.Blessing });
            }
        }
    }
}