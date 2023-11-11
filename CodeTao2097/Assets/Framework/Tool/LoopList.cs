using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace QFramework
{
    [Serializable]
    public class LoopList<T> : IEnumerable<T>
    {
        [SerializeField] private List<T> mItems;
        [SerializeField] private int vernier = 0;
        public int Vernier => vernier;
        public int Count => mItems.Count;
        public bool IsEmpty => mItems.Count == 0;
        public int MaxIndex => mItems.Count - 1;
        public T Last => mItems[MaxIndex];
        public T Current
        {
            get => mItems[vernier];
            set => mItems[vernier] = value;
        }
        public LoopList(int capacity = -1)
        {
            mItems = new List<T>(capacity < 0 ? 2 : capacity);
        }
        public LoopList(List<T> items)
        {
            mItems = items;
        }
        // 索引器
        public T this[int index]
        {
            get => mItems[index];
            set => mItems[index] = value;
        }
        public void VernierToLast() => vernier = MaxIndex;
        public T Find(Predicate<T> match) => mItems.Find(match);
        public void Move(int v) => vernier = v;
        public void Add(T e) => mItems.Add(e);
        public void Push(T e)
        {
            mItems.Add(e);
            LoopPos();
        }
        public bool Remove(T e)
        {
            int index = mItems.IndexOf(e);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }
        public void RemoveAt(int index)
        {
            if (vernier > index) LoopNeg();
            mItems.RemoveAt(index);
        }
        public void LoopNeg()
        {
            vernier = (vernier + MaxIndex) % Count;
        }
        public void LoopPos()
        {
            vernier = (vernier + 1) % Count;
        }
        public override string ToString()
        {
            var res = new StringBuilder();
            int count = mItems.Count;
            int len = count + vernier;
            res.Append($"{GetType().Name}:  Count = {count}   capacity = {mItems.Capacity} \n[");
            for (int i = vernier; i < len; i++)
            {
                res.Append(mItems[i % count]);
                if ((i + 1) % count != vernier) res.Append(",");
            }
            return res.Append("]").ToString();
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < mItems.Count; i++)
            {
                yield return mItems[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < mItems.Count; i++)
            {
                yield return mItems[i];
            }
        }
    }
}