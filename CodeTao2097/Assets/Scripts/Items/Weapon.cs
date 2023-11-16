using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public class Weapon : Item
    {
        public BindableStat attackInterval = new BindableStat(2).SetMinValue(0.1f);

        public Attacker attacker;
        
        public Damager damager;
    }
}