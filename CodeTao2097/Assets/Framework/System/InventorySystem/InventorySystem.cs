using System;

namespace QFramework
{
    public class InventorySystem : AbstractInventorySystem
    {
        protected override void ShowOrHidePanel(Action<bool> callback)
        {
            this.GetSystem<IUGUISystem>().ShowOrHidePanel<BagPanel>(UILayer.Top,false, panel => callback(panel.IsOpen));
        }
    }
}