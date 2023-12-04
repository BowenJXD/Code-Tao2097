using QFramework;

namespace CodeTao
{
    public interface IStackable
    {
        public BindableProperty<int> LVL { get; set; }

        public BindableProperty<int> MaxLVL { get; set; }

        public bool Stack(IStackable newContent)
        {
            Item newItem = (Item) newContent;
            return SetLVL(LVL.Value + newItem.LVL.Value);
        }
        
        public bool SetLVL(int lvl)
        {
            if (lvl > MaxLVL.Value)
            {
                lvl = MaxLVL.Value;
            }
            else if (lvl < 1)
            {
                lvl = 1;
            }
            if (lvl == LVL.Value)
            {
                return false;
            }
            
            LVL.Value = lvl;
            return true;
        }
    }
}