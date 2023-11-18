using System;
using QFramework;

namespace CodeTao
{
    public class ExpController : ViewController
    {
        public BindableProperty<float> EXP = new BindableProperty<float>(0);
        
        public BindableProperty<int> LVL = new BindableProperty<int>(1);

        private void Start()
        {
            EXP.RegisterWithInitValue(exp =>
            {
                float requiredExp = RequiredEXP(LVL.Value);
                if (exp > requiredExp)
                {
                    LevelUp();
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        protected void LevelUp()
        {
            EXP.Value -= RequiredEXP(LVL.Value);
            LVL.Value += 1;
        }
        
        public float RequiredEXP(int level)
        {
            return LVL.Value;
        }
    }
}