using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeTao 
{
    public class Damager : MonoBehaviour
    {
        public BindableStat DMG = new BindableStat();
        public Element DamageElement = new Element();
        
        public Damage ProcessDamage(Damage damage)
        {
            damage.SetMedian(this);
            damage.SetElement(DamageElement);
            damage.SetBase(DMG.Value);
            return damage;
        }
    }

}

