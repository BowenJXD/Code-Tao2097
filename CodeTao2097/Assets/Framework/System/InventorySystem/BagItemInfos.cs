using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace QFramework
{
    [System.Serializable]
    public class BagItemInfos
    {
        [SerializeField] private List<BagItemInfo> mItems;
        // ÿ������ҳ���ʹ�õĸ�������
        [SerializeField] private int mCanUseCellCount;
        // ��ǰҳ
        private int mPageIndex;
        // ҳ������
        private int mPageItemCount;
        public BagItemInfo this[int index]
        {
            get => mItems[index];
            set => mItems[index] = value;
        }
        public int Count => mItems.Count;
        public int CanUseCellCount => mCanUseCellCount;
        public int PageSizeByIndex => mPageItemCount;
        public int CurMinRange => mPageIndex * mPageItemCount;
        public int CurMaxRange => (mPageIndex + 1) * mPageItemCount;
        public int MaxPageNum => mItems.Count / mPageItemCount;
        public int NewPageMaxRange => (MaxPageNum + 1) * mPageItemCount;
        public void ResetPage() => mPageIndex = 0;
        // ��ҳ
        public bool PageTurning(int index)
        {
            // 2 - 0 1
            if (mPageIndex == index) return false;
            bool success = index >= 0 && index <= MaxPageNum;
            if (success) mPageIndex = index;
            return success;
        }
        // ˳��ҳ
        public bool PageTurning(bool isNeg)
        {
            if (mItems.Count <= mPageItemCount) return false;
            int maxPage = MaxPageNum; // 2
            int index = mPageIndex;
            mPageIndex = (mPageIndex + (isNeg ? maxPage - 1 : 1)) % maxPage; // 0 1
            return mPageIndex != index;
        }
        public bool StackOrSwap(int from, int to)
        {
            switch (mItems[to].id)
            {
                case -1:
                    mItems[to] = mItems[from];
                    mItems[from] = BagItemInfo.Empty;
                    break;
                case -2: return false;
                default:
                    var itemFrom = mItems[from];                    
                    mItems[from] = mItems[to];
                    mItems[to] = itemFrom;
                    break;
            }
            return true;
        }
        public bool MoveTo(int from, int to, int StackNum)
        {
            var itemTo = mItems[to];
            var itemFrom = mItems[from];
            if (StackNum == itemTo.count) return false;
            int canStackNum = StackNum - itemTo.count;
            if (itemFrom.count <= canStackNum)
            {
                itemTo.count += itemFrom.count;
                itemFrom.SetEmpty();
            }
            else
            {
                itemTo.count += canStackNum;
                itemFrom.count -= canStackNum;
            }
            mItems[from] = itemFrom;
            mItems[to] = itemTo;
            return true;
        }
        public List<int> AddItem(int id, int count, int StackNum, out int overflow)
        {
            var list = new List<int>();
            overflow = 0;
            // �������ж����Ƿ���Զѵ�            
            for (int i = 0; i < mCanUseCellCount; i++)
            {
                var item = mItems[i];
                if (item.id == id)
                {
                    // ����������ѵ�������ͬ ˵����������
                    if (item.count == StackNum) continue;
                    // ���㵱ǰ���ӵ�ʣ������
                    int canStackNum = StackNum - item.count;
                    // ������������ С�ڵ��� �ɶѵ�����
                    if (count <= canStackNum)
                    {
                        item.count += count;
                        mItems[i] = item;
                        list.Add(i);
                        return list;
                    }
                    // ������������ ������ڿɶѵ�����
                    count -= canStackNum;
                    item.count += canStackNum;
                    mItems[i] = item;
                    list.Add(i);
                }
            }
            for (int i = 0; i < mCanUseCellCount; i++)
            {
                var item = mItems[i];
                // ��������ѵ� �Ϳ�����û�пո���
                if (item.id == -1)
                {
                    if (count <= StackNum)
                    {
                        mItems[i] = new BagItemInfo(id, count);
                        list.Add(i);
                        return list;
                    }
                    // ������������ ������ڿɶѵ�����
                    count -= StackNum;
                    list.Add(i);
                    mItems[i] = new BagItemInfo(id, StackNum);
                }
            }
            overflow = count;
            return list;
        }
        public bool GetItem(int index, out BagItemInfo info)
        {
            if (index >= 0)
            {
                bool success = index < mCanUseCellCount;
                info = success ? mItems[index] : default;
                return success;
            }
            else
            {
                info = default;
                return false;
            }
        }
        public BagItemInfos(int capacity, int CanUseCount)
        {
            mItems = new List<BagItemInfo>(capacity);
            for (int i = 0; i < CanUseCount; i++)
                mItems.Add(BagItemInfo.Empty);
            for (int i = CanUseCount; i < capacity; i++)
                mItems.Add(BagItemInfo.Disable);
            mCanUseCellCount = CanUseCount;
        }
        public void Init(int capacity)
        {
            mPageItemCount = capacity;
        }
        // 25 20 15+25 
        public int Expand(int count)
        {
            int index = mCanUseCellCount;
            mCanUseCellCount += count;
            // �����ǰ�ĵ������� �����ڿ�ʹ�ø�������
            if (mItems.Count >= mCanUseCellCount)
            {
                for (int i = index; i < mCanUseCellCount; i++)
                {
                    mItems[i] = BagItemInfo.Empty;
                }
            }
            else
            {
                for (int i = index; i < mItems.Count; i++)
                {
                    mItems[i] = BagItemInfo.Empty;
                }
                // ���λ�ý��в���
                for (int i = mItems.Count; i < mCanUseCellCount; i++)
                {
                    mItems.Add(BagItemInfo.Empty);
                }
                if (mItems.Count % mPageItemCount != 0)
                {
                    int max = NewPageMaxRange;
                    for (int i = mItems.Count; i < max; i++)
                    {
                        mItems.Add(BagItemInfo.Disable);
                    }
                }
            }
            return index;
        }
        public bool IsInPageRangeByCell(int cellId)
        {
            GetCurPageIndex(ref cellId);
            GetItemRange(out int min, out int max);
            return min <= cellId && (max <= mCanUseCellCount ? max > cellId : mCanUseCellCount - 1 > cellId);
        }
        public bool IsInPageRangeByIndex(int index)
        {
            GetItemRange(out int min, out int max);
            return min <= index && max > index;
        }
        public void GetCurPageIndex(ref int cellId) => cellId += CurMinRange;
        /// <summary>
        /// ��ȡ��ҳ����
        /// </summary>
        public void GetItemRange(out int min, out int max)
        {
            min = CurMinRange;
            max = min + mPageItemCount;
        }
    }
}