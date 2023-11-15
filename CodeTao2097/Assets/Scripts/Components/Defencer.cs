using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class Defencer : ViewController
    {
        public BindableStat DEF = new BindableStat();
        public BindableStat HP = new BindableStat();
        public BindableStat MaxHP = new BindableStat();
        
        public Dictionary<ElementType, float> ElementResistances = ElementType.GetValues(typeof(ElementType))
            .Cast<ElementType>()
            .ToDictionary(key => key, value => 0.0f);
        
        public Damage ProcessDamage(Damage damage)
        {
            damage.SetTarget(this);
            damage.SetDamageSection(DamageSection.TargetDEF, "", DEF.Value);
            damage.SetDamageSection(DamageSection.ElementRES, "", ElementResistances[damage.DamageElement.Type], ERepetitionBehavior.Overwrite);
            return damage;
        }
    }
}

