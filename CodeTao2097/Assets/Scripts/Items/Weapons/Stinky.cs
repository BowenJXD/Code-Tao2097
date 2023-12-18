using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
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