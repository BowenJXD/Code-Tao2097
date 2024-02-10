using System.ComponentModel;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class ScaleController : UnitComponent, IWAtReceiver
    {
        public BindableStat scale = new BindableStat(-1);
        public void OnEnable()
        {
            scale.Init();
            scale.RegisterWithInitValue(value =>
            {
                this.LocalScale(value);
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        private void OnDisable()
        {
            scale.Reset();
        }
        
        public void Receive(IWAtSource source)
        {
            scale.InheritStat(source.GetWAt(EWAt.Area));
        }
    }
}