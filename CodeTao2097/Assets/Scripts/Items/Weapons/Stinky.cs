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
            
            damager = StinkyDamager;

            ats[EWAt.Area].RegisterWithInitValue(range =>
            {
                transform.localScale = Vector3.one * range;
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public override void Fire()
        {
            foreach (var target in GetTargets(StinkyRange))
            {
                Attack(target);
            }
        }
        
        /*public override void Upgrade(int lvlIncrement = 1)
        {
            base.Upgrade(lvlIncrement);
            switch (LVL.Value)
            {
                default:
                    ats[EWAt.Area].AddModifier($"Level{LVL.Value}", 0.3f, EModifierType.MultiAdd, ERepetitionBehavior.AddStack);
                    break;
            }
        }

        public override string GetDescription()
        {
            string result = $"{GetType()}'s range + 30%";
            return result;
        }*/
    }
}