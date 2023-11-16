using System;
using QFramework;

namespace CodeTao
{
    public class ExperienceSystem : ViewController
    {
        public BindableProperty<float> EXP = new BindableProperty<float>(0);
        
        public BindableProperty<int> LVL = new BindableProperty<int>(1);

        private void Start()
        {
            EXP.RegisterWithInitValue(exp =>
            {
                if (exp > RequiredEXP(LVL.Value))
                {
                    EXP.Value = 0;
                    LVL.Value += 1;
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        public float RequiredEXP(int level)
        {
            return LVL.Value;
        }
    }
}