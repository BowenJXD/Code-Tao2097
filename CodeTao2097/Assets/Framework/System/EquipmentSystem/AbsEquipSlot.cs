using System;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 装备插槽基类
    /// 无论是玩家还是敌人 都具备拾取和更换武器的能力
    /// </summary>
    public abstract class AbsEquipSlot : MonoBehaviour
    {
        // [PnReadOnly]
        [SerializeField]
        protected LoopList<EquipInfo> mEquips;
        public event Action<EquipInfo> OnValueChange;
        private void OnDestroy()
        {
            OnValueChange = null;
        }
        public EquipInfo Current => mEquips.Current;
        public void Add(EquipInfo equip) => mEquips.Add(equip);
        public void Remove(int index) => mEquips.RemoveAt(index);
        public void Change(bool isNeg)
        {
            int old = mEquips.Vernier;
            if (isNeg) mEquips.LoopNeg();
            else mEquips.LoopPos();
            // 判断是否变更 如果没用变动无需触发
            if (old == mEquips.Vernier) return;
            OnValueChange?.Invoke(mEquips.Current);
        }
        public EquipInfo this[int index]
        {
            get => mEquips[index];
            set => mEquips[index] = value;
        }
        // 需要在子类定义不同能力槽的逻辑 例如 按A 执行当前武器 xx逻辑
        public void Use(byte abilityIndex)
        {
            Use(mEquips.Current, abilityIndex);
        }
        protected abstract void Use(EquipInfo info, byte abilityIndex);
    }
}