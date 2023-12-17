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
            
            damager = StinkyDamager;

            ats[EWAt.Area].RegisterWithInitValue(range =>
            {
                transform.localScale = Vector3.one * range;
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        /*public override void Fire()
        {
            foreach (var target in GetTargets(StinkyRange))
            {
                Attack(target);
            }
        }*/
    }
}