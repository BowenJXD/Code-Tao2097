using System;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 修为球，用于增加玩家的修为值。为collectable的子类。
    /// </summary>
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