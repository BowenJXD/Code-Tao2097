using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    public class ItemCell : QF_GameController
    {
        public int ID;
        private Image mCellBg;
        private Image mItemImg;
        private Text mItemCount;
        private void Awake()
        {
            mCellBg = GetComponent<Image>();
            mItemImg = transform.Find("ItemSprite").GetComponent<Image>();
            mItemCount = transform.Find("ItemNumText").GetComponentInChildren<Text>();
        }
        public void ChangeSelectState(bool isSelect) => mCellBg.color = isSelect ? Color.red : Color.white;
        public void SetCellBg(Sprite cell)
        {
            mCellBg.sprite = cell;
        }
        public void EnableCell(bool enabled)
        {
            mItemImg.enabled = enabled;
            mItemCount.enabled = enabled;
        }
        public void SetItem(Sprite item, int count)
        {
            mItemImg.sprite = item;
            mItemCount.text = count.ToString();
        }
    }
}