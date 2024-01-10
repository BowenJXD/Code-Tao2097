using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 炼狱Inferno
    /// 类似大蒜，周围一圈。
    /// </summary>
    public partial class Stinky : MeleeWeapon
    {
        public override void Init()
        {
            base.Init();

            damager = ComponentUtil.GetComponentInDescendants<Damager>(this, true);

            ats[EWAt.Area].RegisterWithInitValue(range =>
            {
                transform.localScale = Vector3.one * range;
            }).UnRegisterWhenGameObjectDestroyed(this);
        }
    }
}