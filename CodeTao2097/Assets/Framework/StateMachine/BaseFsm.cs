using UnityEngine;

namespace Panty
{
    public interface ICanPlayAnim
    {
        void PlayAnim(int animHash);
    }
    public abstract class AnimState<T, K> : BaseState<T, K>
        where T : MonoBehaviour, ICanPlayAnim where K : ScriptableObject
    {
        private int mAnimId;
        protected float StateEnterTime;

        public AnimState<T, K> SetAnim(string animName)
        {
            mAnimId = Animator.StringToHash(animName);
            return this;
        }
        protected override void Execute(E_StateLife life)
        {
            if (life != E_StateLife.Enter) return;
            StateEnterTime = Time.time;
            Machine.PlayAnim(mAnimId);
        }
    }
    /// <summary>
    /// 状态生命周期
    /// </summary>
    public enum E_StateLife { Init, Enter, Update, Exit }
    /// <summary>
    /// 基础状态
    /// </summary>
    /// <typeparam name="T">状态处理对象</typeparam>
    /// <typeparam name="K">状态数据对象</typeparam>
    public abstract class BaseState<T, K> : IPnState<E_StateLife>
    {
        private PnState<E_StateLife> _state = new PnState<E_StateLife>();
        protected T Machine { get; private set; }
        protected K Data { get; private set; }
        /// <summary>
        /// 初始化状态需要的数据
        /// </summary>
        /// <param name="machine">状态机交互提供者 一般为Mono</param>
        /// <param name="data">状态机的状态数据 一般使用SO</param>
        public BaseState<T, K> Init(T machine, K data)
        {
            _state.Init(Check, Execute);
            Machine = machine;
            Data = data;
            return this;
        }
        protected abstract string Check();
        protected abstract void Execute(E_StateLife life);
        public BaseState<T, K> SetFather(IPnState<E_StateLife> father)
        {
            _state.SetFather(father);
            return this;
        }
        string IPnState<E_StateLife>.CheckNext()
        {
            return _state.CheckNext();
        }
        void IPnState<E_StateLife>.Trigger(E_StateLife life)
        {
            _state.Trigger(life);
            // if (life == E_StateLife.Enter) Debuger.Log(this.GetType().Name); // 测试代码
        }
    }
    /// <summary>
    /// 基础状态机
    /// </summary>
    public class BaseFsm<T, K> : PnFsm<E_StateLife>
    {
        private T Machine;
        private K Data;
        // 构造函数
        public BaseFsm(T machine, K data)
        {
            Machine = machine;
            Data = data;
        }
        // 设定初始状态的入口为 Enter 方法
        protected override E_StateLife FirstLife => E_StateLife.Enter;
        // 定义状态规则转换
        protected override void SwitchLifeByRule()
        {
            // 这里为了确保 Enter 方法只执行一次 当执行完 Enter 立刻转换到 Update
            if (Life == E_StateLife.Enter) Life = E_StateLife.Update;
        }
        /// <summary>
        /// 状态机构建状态方法
        /// </summary>
        public override void BuildState(IPnStateBuilder<E_StateLife> stateBuilder)
        {
            base.BuildState(stateBuilder);
            InitAllState(state =>
            {
                (state as BaseState<T, K>).Init(Machine, Data);
                state.Trigger(E_StateLife.Init);
            });
        }
        // 变更状态
        protected override void ChangState(IPnState<E_StateLife> nextState)
        {
            CurState.Trigger(E_StateLife.Exit);
            Life = E_StateLife.Enter;
            CurState = nextState;
        }
    }
}
