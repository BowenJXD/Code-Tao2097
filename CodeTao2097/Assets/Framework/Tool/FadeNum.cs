using System;

namespace QFramework
{
    public enum FadeState
    {
        Close,//关闭
        FadeIn,
        FadeOut,
    }
    /// <summary>
    ///数字渐变动画
    /// </summary>
    public class FadeNum
    {
        /// <summary>
        /// 淡入状态
        /// </summary>
        private FadeState mFadeState = FadeState.Close;
        /// <summary>
        /// 是否启动
        /// </summary>
        public bool IsFinish => mFadeState == FadeState.Close;
        /// <summary>
        /// 调用委托的时候避免提前结束
        /// </summary>
        private bool mInit = false;
        /// <summary>
        /// 淡入结束后要做的事情
        /// </summary>
        private Action mOnEvent;
        /// <summary>
        /// 当前值
        /// </summary>
        private float mCurrentValue;
        /// <summary>
        /// 最大最小范围
        /// </summary>
        private float mMin = 0, mMax = 1;
        /// <summary>
        /// 设置范围
        /// </summary>
        public void SetMinMax(float min, float max)
        {
            mMin = min;
            mMax = max;
        }
        /// <summary>
        /// 设置状态
        /// </summary>
        public void SetState(FadeState state, Action action = null)
        {
            mOnEvent = action;
            mFadeState = state;
            mInit = false;
        }
        /// <summary>
        /// 需要再Update持续检测
        /// </summary>
        public float Update(float step)
        {
            switch (mFadeState)
            {
                //如果是渐入状态 0 - 1
                case FadeState.FadeIn:
                    //确认初始化参数
                    if (!mInit) Init(mMin);
                    // 变更值
                    if (mCurrentValue < mMax)
                    {
                        mCurrentValue += step;
                    }
                    else OnEnd(mMax);
                    break;
                //如果是渐出状态 1 - 0
                case FadeState.FadeOut:
                    //确认初始化参数
                    if (!mInit) Init(mMax);
                    // 变更值
                    if (mCurrentValue > mMin)
                    {
                        mCurrentValue -= step;
                    }
                    else OnEnd(mMin);
                    break;
            }
            return mCurrentValue;
        }

        private void OnEnd(float value)
        {
            mOnEvent?.Invoke();
            mCurrentValue = value;
            if (mInit) mFadeState = FadeState.Close;
        }

        private void Init(float value)
        {            
            mCurrentValue = value;
            mInit = true;
        }
    }
}