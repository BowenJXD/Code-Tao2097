using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public partial class Stinky : Weapon
    {
        protected override void Start()
        {
            base.Start();
            
            damager = Damager;
        }

        public override void Fire()
        {
            foreach (var target in GetTargets())
            {
                Attack(target);
            }
        }
    }
}