using System;
using QFramework;

namespace CodeTao
{
    public class ExpUpgradeItem : Item
    {
        public ExpUpgradeItem(bool isWeapon)
        {
            IsWeapon = isWeapon;
        }
        
        // 是否是武器
        public bool IsWeapon = false;
        public bool UpgradeFinish => LVL.Value >= MaxLVL.Value;
        public string Key { get; private set; }
        public string Name { get; private set; }
        public string Description => mDescriptionFactory(LVL.Value + 1);

        public string IconName { get; private set; }

        public BindableProperty<bool> Visible = new BindableProperty<bool>();
        private Func<int, string> mDescriptionFactory;
        
        

        public void Upgrade()
        {
            LVL.Value++;
            mOnUpgrade?.Invoke(this,LVL.Value);
            // ExpUpgradeSystem.CheckAllUnlockedFinish();
        }


        private Action<ExpUpgradeItem,int> mOnUpgrade;

        public ExpUpgradeItem OnUpgrade(Action<ExpUpgradeItem,int> onUpgrade)
        {
            mOnUpgrade = onUpgrade;
            return this;
        }
        
        public ExpUpgradeItem WithKey(string key)
        {
            Key = key;
            return this;
        }

        public ExpUpgradeItem WithName(string  name)
        {
            Name = name;
            return this;
        }
        
        public ExpUpgradeItem WithIconName(string iconName)
        {
            IconName = iconName;
            return this;
        }
        
        public string PairedName { get; private set; }
        public string PairedDescription { get; private set; }
        public string PairedIconName { get; private set; }

        public ExpUpgradeItem WithPairedName(string pairedName)
        {
            PairedName = pairedName;
            return this;
        }
        
        public ExpUpgradeItem WithPairedIconName(string pairedIconName)
        {
            PairedIconName = pairedIconName;
            return this;
        }

        public ExpUpgradeItem WithPairedDescription(string pairedDescription)
        {
            PairedDescription = pairedDescription;
            return this;
        }
        
        public ExpUpgradeItem WithDescription(Func<int,string> descriptionFactory)
        {
            mDescriptionFactory = descriptionFactory;
            return this;
        }

        public ExpUpgradeItem WithMaxLevel(int maxLevel)
        {
            MaxLVL.Value = maxLevel;
            return this;
        }
    }
}