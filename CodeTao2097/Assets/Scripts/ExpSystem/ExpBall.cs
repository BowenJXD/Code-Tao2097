using System;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public partial class ExpBall : Collectable
    {
        public BindableProperty<float> EXPValue = new BindableProperty<float>(1);
        
        public override void Interact(Collider2D col = null){
            ExpController expController = col.GetComponentFromUnit<ExpController>();
            expController.AlterEXP(EXPValue.Value);
            base.Interact(col);
        }
    }
}