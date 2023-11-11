using System;
using System.Collections.Generic;

namespace Panty
{
    /// <summary>
    /// 状态接口
    /// </summary>
    public interface IPnState<T> where T : Enum
    {
        void Trigger(T life);
        string CheckNext();
    }
    /// <summary>
    /// 基础状态
    /// </summary>
    public class PnState<T> : IPnState<T> where T : Enum
    {
        // 父类状态
        private IPnState<T> mFather;
        private Func<string> OnCheck;
        private Action<T> OnExecute;
        /// <summary>
        /// 初始化行为
        /// </summary>
        public void Init(Func<string> check, Action<T> execute)
        {
            OnCheck = check;
            OnExecute = execute;
        }
        /// <summary>
        /// 设置状态父级
        /// </summary>
        public void SetFather(IPnState<T> father) => mFather = father;
        /// <summary>
        /// 触发更新事件
        /// </summary>
        public void Trigger(T life)
        {
            mFather?.Trigger(life);
            OnExecute?.Invoke(life);
        }
        /// <summary>
        /// 状态条件检测
        /// </summary>
        public string CheckNext()
        {
            string nextStateName = mFather?.CheckNext();
            if (nextStateName == null)
            {
                nextStateName = OnCheck?.Invoke();
            }
            return nextStateName;
        }
    }
    /// <summary>
    /// 功能插槽接口
    /// </summary>
    public interface IFunctionSlot
    {
        void Execute();
    }
    /// <summary>
    /// 状态构建器
    /// </summary>
    public interface IPnStateBuilder<T> where T : Enum
    {
        Dictionary<string, IPnState<T>> Create(out string firstStateName, out IGlobalStateEvent globalEvent);
    }
    /// <summary>
    /// 全局状态事件
    /// </summary>
    public interface IGlobalStateEvent
    {
        bool CanDo { get; }
        string Check();
    }
    /// <summary>
    /// 主状态机
    /// </summary>
    public abstract class PnFsm<T> where T : Enum
    {
        // 生命周期
        protected T Life;
        // 当前状态
        protected IPnState<T> CurState;
        // 状态列表
        private Dictionary<string, IPnState<T>> mStates;
        // 功能插槽组
        private List<IFunctionSlot> mFuncSlots;
        // 状态全局事件
        private IGlobalStateEvent mGlobalEvent;
        /// <summary>
        /// 链接功能插槽
        /// </summary>
        public PnFsm<T> LinkSlot(IFunctionSlot slot)
        {
            if (mFuncSlots == null)
                mFuncSlots = new List<IFunctionSlot>();
            mFuncSlots.Add(slot);
            return this;
        }
        /// <summary>
        /// 构建状态 [1]
        /// </summary>
        public virtual void BuildState(IPnStateBuilder<T> stateBuilder)
        {
            Life = FirstLife;
            // 为了将状态构建拆分成单独的类 使用了 IStateBuilder 接口 对外提供一个方法用于构建           
            mStates = stateBuilder.Create(out string firstStateName, out mGlobalEvent);
            // 如果无法找到 初始状态 就抛出一个错误
            if (!mStates.TryGetValue(firstStateName, out CurState))
                throw new Exception("状态不存在,请检查状态输入是否正确！");
        }
        /// <summary>
        /// 初始化状态列表和当前状态 [2]
        /// </summary>
        protected void InitAllState(Action<IPnState<T>> callback)
        {
            if (mStates == null || mStates.Count == 0) return;
            foreach (var state in mStates.Values) callback(state);
        }
        private void ChangState(string nextStateName)
        {
            if (nextStateName == null) return;
            // 状态转换前 需要确认一下状态是否存在
            if (!mStates.TryGetValue(nextStateName, out var state))
                throw new Exception("状态不存在,请检查状态输入是否正确！");
            // 如果状态验证成功 执行状态转换
            ChangState(state);
        }
        // 转换生命周期规则
        protected abstract void SwitchLifeByRule();
        // 状态转换
        protected abstract void ChangState(IPnState<T> nextState);
        // 子类重写 首次执行的生命周期
        protected abstract T FirstLife { get; }
        /// <summary>
        /// 更新函数
        /// </summary>
        public void Update()
        {
            if (CurState == null) return;
            // 例如死亡状态 就不让他执行全局事件
            if (mGlobalEvent != null && mGlobalEvent.CanDo)
            {
                // 如果功能插槽有东西 执行功能列表所有功能
                if (mFuncSlots != null)
                    for (int i = 0; i < mFuncSlots.Count; i++) mFuncSlots[i].Execute();
                // 全局检测下一个状态 如果新的状态名字 不为空 转换状态
                ChangState(mGlobalEvent.Check());
            }
            // 在 Update 中 执行更新逻辑
            CurState.Trigger(Life);
            // 更新后 转换生命周期
            SwitchLifeByRule();
            // 判断是否转换当前状态 如果下一个状态不为空 就转换状态
            ChangState(CurState.CheckNext());
        }
        /// <summary>
        /// 释放状态机
        /// </summary>
        public void Dispose()
        {
            mStates.Clear();
            mStates = null;
            CurState = null;
            mGlobalEvent = null;
            mFuncSlots = null;
        }
    }
}