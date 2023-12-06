using System.Collections.Generic;
using QFramework;

namespace CodeTao
{
    public class ExpUpgradeSystem : AbstractSystem
    {
        public List<ExpUpgradeItem> Items { get; } = new List<ExpUpgradeItem>();
        
        protected override void OnInit()
        {
            
        }
        
        public ExpUpgradeItem Add(ExpUpgradeItem item)
        {
            Items.Add(item);
            return item;
        }
    }
}