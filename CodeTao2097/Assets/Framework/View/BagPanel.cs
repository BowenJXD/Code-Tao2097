using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace QFramework
{
    public class BagPanel : QF_UIController, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private Sprite mEmptyCell;
        [SerializeField] private Sprite mDisableCell;

        private List<ItemCell> mCells;
        private Dictionary<string, E_BagItemType> mPages;

        private int vernier;

        private ItemCell mSelectCell;
        private ItemSelector mItemSelector;
        private Image mSelectItemIcon;
        private Text mCoinNumTex;

        private int rowsNum;
        private int pageSize;

        private IInventorySystem mInventory;

        protected override void Awake()
        {
            base.Awake();

            mInventory = this.GetSystem<IInventorySystem>();

            mItemSelector = GetComponentInChildren<ItemSelector>();
            mSelectItemIcon = transform.Find("SelectItemIcon").GetComponent<Image>();
            mSelectItemIcon.enabled = false;

            this.FindChildrenControl<Text>((name, control) =>
            {
                if (name == "CoinNum")
                {
                    mCoinNumTex = control;
                }
            });

            this.RegisterEvent<SwitchBagPagesEvent>(e => MoveCellSelector(e.index % pageSize)).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.GetModel<IPlayerResModel>().CoinNum.RegisterWithInitValue(num => mCoinNumTex.text = num.ToString()).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<ItemChangeEvent>(e =>
            {
                var cell = mCells[e.index % pageSize];
                cell.EnableCell(e.info.id >= 0);
                switch (e.info.id)
                {
                    case -1:
                        cell.SetCellBg(mEmptyCell);
                        break;
                    case -2:
                        cell.SetCellBg(mDisableCell);
                        break;
                    default:
                        cell.SetItem(mInventory.GetItemConfig(e.info).sprite, e.info.count);
                        cell.SetCellBg(mEmptyCell);
                        break;
                }
            })
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            this.RegisterEvent<DirInputEvent>(OnDirInput);
            this.RegisterEvent<InteractiveKeyInputEvent>(OnInteractiveKeyInput);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            this.UnRegisterEvent<DirInputEvent>(OnDirInput);
            this.UnRegisterEvent<InteractiveKeyInputEvent>(OnInteractiveKeyInput);
        }
        private void OnInteractiveKeyInput(InteractiveKeyInputEvent e)
        {
            if (mSelectCell == null)
            {
                if (mInventory.GetItemByCell(mCells[vernier].ID, out var info) && info.id >= 0)
                {
                    mSelectCell = mCells[vernier];
                    mSelectCell.ChangeSelectState(true);
                }
            }
            else
            {
                // µ±Ç°ID±ØÐëÓëÓÎ±êID²»Í¬ ²Å×ö´¦Àí
                if (mSelectCell.ID.Equals(vernier))
                {
                    mInventory.RemoveItemByCell(mSelectCell.ID);
                }
                else
                {
                    mInventory.MoveItemByCell(mSelectCell.ID, vernier);
                }
                mSelectCell.ChangeSelectState(false);
                mSelectCell = null;
            }
        }
        private void OnDirInput(DirInputEvent e)
        {
            var dir = e.dir;
            // ÒÆ¶¯Ñ¡Ôñ¿ò ÎÞÊäÈëÊ± ²»½øÐÐ´¦Àí
            // if (dir.Equals(Vector2.zero)) return;
            var absX = Mathf.Abs(dir.x);
            var absY = Mathf.Abs(dir.y);
            // µ±·½ÏòµÄXY¶¼ÓÐÖµÊ±ËµÃ÷Ê±Ð±·½Ïò ²»ÔÊÐí
            // if (absX > 0 && absY > 0) return;
            // µ±X·½ÏòÓÐÖµ Y ·½ÏòÃ»ÓÐÖµÊ±
            if (absX > 0 && absY < 0.1f)
            {
                if (dir.x > 0)
                {
                    // Ð¡ÓÚµ±Ç°Ò³µÄ¿ÉÊ¹ÓÃÊýÁ¿
                    if (mInventory.IsInPageRange(vernier))
                    {
                        MoveCellSelector(vernier + 1);
                    }
                }
                else
                {
                    var next = vernier - 1;
                    if (next >= 0) MoveCellSelector(next);
                }
            }
            // ÕâÀïµÄ 0.1 Ö÷ÒªÔÚÓÅ»¯ÊÖ±ú
            else if (absX < 0.1f && absY > 0)
            {
                if (dir.y > 0)
                {
                    var next = vernier - rowsNum;
                    if (next >= 0) MoveCellSelector(next);
                }
                else
                {
                    var next = vernier + rowsNum;
                    if (mInventory.IsInPageRange(next - 1))
                    {
                        MoveCellSelector(next);
                    }
                }
            }
        }
        protected override void OnValueChanged(string togName, bool isOn)
        {
            if (isOn && mPages.TryGetValue(togName, out var type))
            {
                mInventory.SwitchType(type);
            }
        }
        protected override void OnClick(string btnName)
        {
            this.GetSystem<IUGUISystem>().HidePanel<BagPanel>(true);
        }
        public override void ShowMe()
        {
            base.ShowMe();
            // ½«Ñ¡ÔñÆ÷ÒÆ¶¯µ½³õÊ¼Î»ÖÃ
            vernier = 0;
            // ÖØÖÃÑ¡ÔñÆ÷Î»ÖÃ
            mItemSelector.ResetPos();
            // Èç¹û¸ñ×ÓÊý×éÎª¿Õ ËµÃ÷µÚÒ»´Î¼ÓÔØ±³°ü ÐèÒª½øÐÐ³õÊ¼»¯
            if (mCells == null && mInventory.GetOrCreateItemConfig(out var config))
            {
                pageSize = config.PageSize;
                rowsNum = config.RowCount;
                // ¼ÆËãÃ¿Ò³µÄ¹Ì¶¨ÊýÁ¿ ³õÊ¼»¯¸ñ×ÓÁÐ±í
                mCells = new List<ItemCell>(pageSize);
                // ³õÊ¼»¯Ö÷ÈÝÆ÷
                var itemBg = transform.Find("ItemBg");
                var itemList = itemBg.Find("ItemList") as RectTransform;
                var itemView = itemList.Find("ItemView") as RectTransform;
                itemView.sizeDelta = itemList.sizeDelta = config.ViewSize;

                mPages = new Dictionary<string, E_BagItemType>();
                PEnum.Loop<E_BagItemType>(type =>
                {
                    if ((type & config.types) == 0) return;
                    mPages.Add(type.ToString(), type);
                });
                var bagTogGroup = itemBg.Find("BagPageTog") as RectTransform;
                if (mPages.Count <= 1)
                {
                    itemList.anchoredPosition += Vector2.up * bagTogGroup.sizeDelta.y;
                    GameObject.Destroy(bagTogGroup.gameObject);
                    mPages = null;
                }
                else
                {
                    bagTogGroup.sizeDelta = new Vector2(itemList.sizeDelta.x, bagTogGroup.sizeDelta.y);
                    bagTogGroup.anchoredPosition = new Vector2(0, bagTogGroup.anchoredPosition.y);
                    var tempTog = bagTogGroup.Find("Toggle") as RectTransform;
                    float step = bagTogGroup.sizeDelta.x / mPages.Count;
                    tempTog.sizeDelta = new Vector2(step, tempTog.sizeDelta.y);
                    int num = 0;
                    var togs = new List<Toggle>(mPages.Count - 1);
                    Toggle _tog = null;
                    foreach (var type in mPages.Values)
                    {
                        var typeStr = type.ToString();
                        var o = num == 0 ? tempTog.gameObject : GameObject.Instantiate(tempTog.gameObject, bagTogGroup);
                        var togTrans = o.transform as RectTransform;
                        togTrans.anchoredPosition = new Vector2(step * num++, tempTog.anchoredPosition.y);
                        o.transform.GetChild(0).name = typeStr + "Bg";
                        o.name = typeStr;
                        var textCtrl = o.GetComponentInChildren<Text>();
                        textCtrl.name = typeStr + "Text";
                        textCtrl.text = typeStr;
                        var tog = o.GetComponent<Toggle>();
                        tog.onValueChanged.AddListener(isOn => OnValueChanged(typeStr, isOn));
                        if (type == config.defaultType)
                        {
                            _tog = tog;
                        }
                        else togs.Add(tog);
                    }
                    if (_tog == null)
                        throw new System.Exception($"{config.defaultType}Ã»ÓÐÔÚÅäÖÃÃ¶¾ÙÁÐ±íÖÐ");
                    var group = bagTogGroup.GetComponent<ToggleGroup>();
                    _tog.group = group;
                    _tog.isOn = true;
                    for (int i = 0; i < togs.Count; i++)
                    {
                        togs[i].group = group;
                    }
                }
                // ³õÊ¼»¯Ñ¡ÔñÆ÷´óÐ¡
                var cellSize = config.CellSize;
                mItemSelector.InitSize(cellSize);
                // ¸ù¾ÝÊý¾Ý ´´½¨¶ÔÓ¦ÊýÁ¿¸ñ×Ó
                for (int i = 0; i < pageSize; i++)
                {
                    var o = ResHelper.SyncLoad<GameObject>("Prefabs/ItemCell");
                    o.transform.SetParent(itemView);
                    var cellTrans = o.transform as RectTransform;
                    cellTrans.sizeDelta = cellSize;
                    cellTrans.anchoredPosition = config.GetPos(i);
                    cellTrans.localScale = Vector2.one;
                    // ¸üÐÂ¸ñ×ÓÊý¾Ý
                    var cell = o.GetComponent<ItemCell>();
                    cell.ID = mCells.Count;
                    mCells.Add(cell);
                }
            }
        }
        private void MoveCellSelector(int targetIndex)
        {
            // Èç¹ûIDÏàÍ¬ ²»½øÐÐÒÆ¶¯
            if (vernier == targetIndex) return;
            // È·±£Ë÷ÒýºÏ·¨
            if (mItemSelector.CanStartMove)
            {
                vernier = targetIndex;
                mItemSelector.SetTarget(mCells[vernier].transform);
            }
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData e)
        {
            // ±ØÐëÊÇ×ó¼ü°´ÏÂ
            if (e.button == PointerEventData.InputButton.Left)
            {
                // ±ØÐëÊÇÓÐ¶ÔÏó²ÅÄÜ²Ù×÷
                var o = e.pointerPressRaycast.gameObject;
                mSelectCell = o?.GetComponent<ItemCell>();
                if (mSelectCell == null) return;
                // ²éÑ¯IDÄÚÈÝ
                if (mInventory.GetItemByCell(mSelectCell.ID, out var info) && info.id >= 0)
                {
                    mSelectItemIcon.enabled = true;
                    mSelectCell.EnableCell(false);
                    mSelectItemIcon.sprite = mInventory.GetItemConfig(info).sprite;
                    mSelectItemIcon.transform.position = e.position;
                }
                else
                {
                    mSelectCell = null;
                }
            }
        }
        void IDragHandler.OnDrag(PointerEventData e)
        {
            if (mSelectCell == null) return;
            mSelectItemIcon.transform.position = e.position;
        }
        void IPointerUpHandler.OnPointerUp(PointerEventData e)
        {
            if (mSelectCell == null) return;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                var cell = e.pointerCurrentRaycast.gameObject.GetComponent<ItemCell>();
                if (cell != null)
                {
                    if (cell == mSelectCell)
                    {
                        mInventory.ResetItemByCell(mSelectCell.ID);
                    }
                    else
                    {
                        mInventory.MoveItemByCell(mSelectCell.ID, cell.ID);
                    }
                }
            }
            else
            {
                mInventory.RemoveItemByCell(mSelectCell.ID);
            }
            mSelectItemIcon.enabled = false;
            mSelectCell = null;
        }
    }
}