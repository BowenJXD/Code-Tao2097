using Panty;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    [Serializable]
    public class BagItemSave : IArchive
    {
        public E_BagItemType[] itemTypes;
        public BagItemInfos[] Infos;
    }
    [Flags]
    public enum E_BagItemType : byte
    {
        Weapon = 1 << 0,
        Consumables = 1 << 1,
        Other = 1 << 2,
        Empty = 1 << 3
    }
    [Serializable]
    public class BagItemConfig
    {
        [PnShowSprite]
        public Sprite sprite;
        public string name;
        public int stackNum = 1;
        public string description;
    }
    [Serializable]
    public struct BagItemInfo
    {
        public static readonly BagItemInfo Empty = new BagItemInfo(-1, 0);
        public static readonly BagItemInfo Disable = new BagItemInfo(-2, 0);

        public int id;
        public int count;

        public BagItemInfo(int id, int count)
        {
            this.id = id;
            this.count = count;
        }
        public void SetEmpty() => id = -1;
    }
    public struct ItemChangeEvent
    {
        public int index;
        public BagItemInfo info;
    }
    public struct SwitchBagPagesEvent
    {
        public int index;
    }
    public interface IInventorySystem : ISystem
    {
        void ShowOrHide();
        bool IsInPageRange(int index);
        void SwitchPages();
        void SwitchType(E_BagItemType type);
        void ExpandCapacity(E_BagItemType type, int count);
        void ExpandCapacity(int count);
        void AddItem(int id, int count = 1);
        void MoveItem(int from, int to);
        void MoveItemByCell(int from, int to);
        void RemoveItem(int index);
        void RemoveItemByCell(int cellId);
        void RemoveItem(int index, int count);
        void RemoveItemByCell(int cellId, int count);
        void ResetItemByCell(int cellId);
        bool GetItem(int index, out BagItemInfo info);
        bool GetItemByCell(int cellId, out BagItemInfo info);
        BagItemConfig GetItemConfig(BagItemInfo info);
        bool GetOrCreateItemConfig(out SO_BagItemConfig configSo);
    }
    // 库存系统
    public abstract class AbstractInventorySystem : AbstractSystem, IInventorySystem, ISaveMark<BagItemSave>
    {
        #region 数据部分        
        // 道具配置字典
        private Dictionary<E_BagItemType, BagItemConfig[]> mConfigs;
        // 道具列表
        protected Dictionary<E_BagItemType, BagItemInfos> mItemDic;
        // 道具变更事件
        private ItemChangeEvent mOnItemChange;
        // 当前选中的道具列表
        private BagItemInfos mCurItems;
        // 当前道具表的类型
        private E_BagItemType mCurType;
        // 判断面板是否显示
        private bool mIsOpen;
        // 脏标记
        private bool mDirtyFlag = true;

        bool IInventorySystem.IsInPageRange(int index) => mCurItems.IsInPageRangeByCell(index);
        #endregion

        #region 重写方法
        protected abstract void ShowOrHidePanel(Action<bool> callback);
        protected override void OnInit()
        {
            this.GetSystem<IArchiveSystem>().Add(this);
        }
        #endregion

        #region 存档标记
        void ISaveMark<BagItemSave>.Save(BagItemSave archive)
        {
            if (mItemDic == null || mItemDic.Count == 0) return;
            archive.Infos = new BagItemInfos[mItemDic.Count];
            int index = 0;
            foreach (var item in mItemDic)
            {
                archive.itemTypes[index] = item.Key;
                archive.Infos[index++] = item.Value;
            }
        }
        void ISaveMark<BagItemSave>.Load(BagItemSave archive)
        {
            if (archive.Infos == null) return;
            int len = archive.Infos.Length;
            mItemDic = new Dictionary<E_BagItemType, BagItemInfos>(len);
            for (int i = 0; i < len; i++)
            {
                mItemDic.Add(archive.itemTypes[i], archive.Infos[i]);
            }
        }
        #endregion

        #region 接口方法
        public bool GetOrCreateItemConfig(out SO_BagItemConfig configSo)
        {
            if(SerializeHelper.TryGetSO("BagItemConfig", out configSo)) return true;
            SerializeHelper.ShowCreateTips<SO_BagItemConfig>("BagItemConfig");
            return false;
        }
        BagItemConfig IInventorySystem.GetItemConfig(BagItemInfo info)
        {
            return info.id >= 0 && mConfigs.TryGetValue(mCurType, out var configs) ? configs[info.id] : null;
        }
        void IInventorySystem.ShowOrHide()
        {
            // 如果是第一次开启面板 初始化页面
            if (mCurItems == null && GetOrCreateItemConfig(out var configSo))
            {
                // 缓存初始状态
                bool loadFailure = mItemDic == null;
                // 检查存档是否读取成功 如果打开时背包还为空 说明未成功加载存档 创建道具字典
                if (loadFailure) mItemDic = new Dictionary<E_BagItemType, BagItemInfos>();
                mConfigs = new Dictionary<E_BagItemType, BagItemConfig[]>();
                // 初始化道具配置表 
                PEnum.Loop<E_BagItemType>(type =>
                {
                    if ((type & configSo.types) == 0) return;
                    string soName = $"SO_{type}ItemConfig";
                    if (SerializeHelper.TryGetSO<SO_BagItemDatas>(soName, out var so))
                    {
                        if (so.configs == null) return;
                        // 注意配置时不要将堆叠数量设置为 0
                        int pageSize = configSo.PageSize;
                        if (loadFailure) mItemDic[type] =
                            new BagItemInfos(pageSize, configSo.defaultCanUseCount);
                        mConfigs.Add(type, so.configs);
                        mItemDic[type].Init(pageSize);
                    }
                    else
                    {
                        SerializeHelper.ShowCreateTips<SO_BagItemDatas>(soName);
                    }
                });
            }
            // 执行开关面板命令
            ShowOrHidePanel(open =>
            {
                mIsOpen = open;
                // 如果是开启背包面板 更新当前显示的道具
                if (mIsOpen && mDirtyFlag)
                {
                    mDirtyFlag = false;
                    RefreshPage();
                }
            });
        }
        void IInventorySystem.ExpandCapacity(int count)
        {
            ExpandCapacity(mCurType, count);
        }
        /// <summary>
        /// 给背包的某个页面扩容
        /// </summary>
        public void ExpandCapacity(E_BagItemType type, int count)
        {
            if (mItemDic.TryGetValue(type, out var infos))
            {
                // 缓存扩容前的可使用数量 扩容后 得更新UI
                int canUseIndex = infos.Expand(count);
                // 如果背包处于打开状态 并且背包页处于显示状态
                if (mIsOpen)
                {
                    if (type == mCurType)
                    {
                        int max = infos.CurMaxRange;
                        if (canUseIndex >= max) return;
                        for (int i = canUseIndex; i < max; i++)
                        {
                            SendItemChangeEvent(i);
                        }
                    }
                }
                // 如果没有打开背包 说明下次开启需要更新
                else mDirtyFlag = true;
            }
        }
        public void SwitchType(E_BagItemType type)
        {
            if (type == E_BagItemType.Empty)
                throw new Exception("枚举值或枚举名不正确");
            if (type == mCurType) return;
            if (mItemDic.TryGetValue(type, out var infos))
            {
                mCurType = type;
                mCurItems = infos;
                mCurItems.ResetPage();
                // 发送当前页面道具信息来更新UI
                if (mIsOpen)
                {
                    int len = mCurItems.PageSizeByIndex;
                    for (int i = 0; i < len; i++)
                    {
                        SendItemChangeEvent(i);
                    }
                }
                else mDirtyFlag = true;
            }
        }
        void IInventorySystem.SwitchPages()
        {
            if (mIsOpen && mCurItems.PageTurning(false))
            {
                this.SendEvent(new SwitchBagPagesEvent() { index = mCurItems.CurMinRange });
                RefreshPage();
            }
        }

        private void RefreshPage()
        {
            if (mCurItems == null) return;
            mCurItems.GetItemRange(out int min, out int max);
            for (int i = min; i < max; i++)
            {
                SendItemChangeEvent(i);
            }
        }
        #endregion

        #region 增删改查
        void IInventorySystem.ResetItemByCell(int cellId)
        {
            mCurItems.GetCurPageIndex(ref cellId);
            TrySendItemChangeEvent(cellId);
        }
        // 从当前的页面中获取一个元素
        bool IInventorySystem.GetItem(int index, out BagItemInfo info) =>
            mCurItems.GetItem(index, out info);
        bool IInventorySystem.GetItemByCell(int cellId, out BagItemInfo info)
        {
            mCurItems.GetCurPageIndex(ref cellId);
            return mCurItems.GetItem(cellId, out info);
        }
        /// <summary>
        /// 移动道具
        /// </summary>
        /// <param name="from">从哪个索引的格子来</param>
        /// <param name="to">到某个索引的格子去</param>
        void IInventorySystem.MoveItemByCell(int from, int to)
        {
            mCurItems.GetCurPageIndex(ref from);
            mCurItems.GetCurPageIndex(ref to);
            MoveItem(from, to);
        }
        /// <summary>
        /// 移除指定数量的道具
        /// </summary>
        /// <param name="cellId">移除的索引位置</param>
        /// <param name="count">移除的数量</param>
        void IInventorySystem.RemoveItemByCell(int cellId, int count)
        {
            mCurItems.GetCurPageIndex(ref cellId);
            RemoveItem(cellId, count);
        }
        /// <summary>
        /// 添加道具
        /// </summary>
        /// <param name="id">添加的道具ID</param>
        /// <param name="count">添加的道具数量</param>
        void IInventorySystem.AddItem(int id, int count)
        {
            if (mConfigs.TryGetValue(mCurType, out var configs))
            {
                var stackNum = configs[id].stackNum;
                if (stackNum == 0) return;
                var list = mCurItems.AddItem(id, count, stackNum, out int overflow);
                if (list.Count == 0) return;
                if (mIsOpen)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (mCurItems.IsInPageRangeByIndex(list[i]))
                        {
                            SendItemChangeEvent(list[i]);
                        }
                    }
                }
                else mDirtyFlag = true;
            }
        }
        /// <summary>
        /// 移除某个索引所有道具
        /// </summary>
        /// <param name="cellId">道具索引</param>
        void IInventorySystem.RemoveItemByCell(int cellId)
        {
            mCurItems.GetCurPageIndex(ref cellId);
            RemoveItem(cellId);
        }
        public void RemoveItem(int index)
        {
            mCurItems[index] = BagItemInfo.Empty;
            TrySendItemChangeEvent(index);
        }
        public void MoveItem(int from, int to)
        {
            // 如果两个格子索引相同 将拾取的fromID进行刷新 if 目标大于可使用数量
            if (from == to || to >= mCurItems.CanUseCellCount)
            {
                TrySendItemChangeEvent(from);
                return;
            }
            var itemFrom = mCurItems[from];
            // 如果第一个id是空或者是封闭的 那么没必要更新
            if (itemFrom.id < 0) return;
            // 这里能够确定拿过来一个有效的id
            var itemTo = mCurItems[to];
            // 看看两个ID是否相同 这里能保证id都是有效的
            if (itemFrom.id == itemTo.id)
            {
                if (mConfigs.TryGetValue(mCurType, out var config))
                {
                    bool isRefresh = mCurItems.MoveTo(from, to, config[itemTo.id].stackNum);
                    if (mIsOpen)
                    {
                        SendItemChangeEvent(from);
                        if (isRefresh) SendItemChangeEvent(to);
                    }
                    else mDirtyFlag = true;
                }
            }
            else if (mCurItems.StackOrSwap(from, to))
            {
                if (mIsOpen)
                {
                    SendItemChangeEvent(from);
                    SendItemChangeEvent(to);
                }
                else mDirtyFlag = true;
            }
            else TrySendItemChangeEvent(from);
        }
        public void RemoveItem(int index, int count)
        {
            var item = mCurItems[index];
            // 如果需要移除的数量 大于已有数量
            if (item.count <= count)
            {
                item.SetEmpty();
            }
            else
            {
                item.count -= count;
            }
            mCurItems[index] = item;
            TrySendItemChangeEvent(index);
        }
        #endregion

        #region 私有方法
        private void TrySendItemChangeEvent(int index)
        {
            if (mIsOpen) SendItemChangeEvent(index);
            else mDirtyFlag = true;
        }
        private void SendItemChangeEvent(int index)
        {
            mOnItemChange.index = index;
            mOnItemChange.info = mCurItems[index];
            this.SendEvent(mOnItemChange);
            Log(index);
        }
        private void Log(int index)
        {
            string msg = string.Empty;
            var info = mCurItems[index];
            if (info.id >= 0)
            {
                if (mConfigs.TryGetValue(mCurType, out var configs))
                {
                    msg = $"格子{index}变更为{configs[info.id].name}";
                }
            }
            else
            {
                msg = $"格子{index}变更为空";
            }
            this.GetSystem<IMessageSystem>().Log(msg);
        }
        #endregion
    }
}