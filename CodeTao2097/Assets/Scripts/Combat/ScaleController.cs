using System.ComponentModel;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class ScaleController : UnitComponent, IWAtReceiver
    {
        public BindableStat scale = new BindableStat(1);
        public MonoBehaviour controlee;
        public void OnEnable()
        {
            if (!controlee) controlee = Unit;
            scale.Init();
            scale.RegisterWithInitValue(value =>
            {
                controlee.LocalScale(value);
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