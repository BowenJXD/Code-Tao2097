using System;
using UnityEngine;
using UnityEngine.Events;

namespace CodeTao
{
    /// <summary>
    /// 武器属性修改器，函数需要手动调用。
    /// </summary>
    public class WAtModder : MonoBehaviour
    {
        public EWAt at;
        public float modValue;
        public EModifierType modType;
        public RepetitionBehavior repetitionBehavior;
        
        public Weapon source;

        public void AddMod()
        {
            source.GetWAt(at).AddModifier(modValue, modType, name, repetitionBehavior);
        }

        public void RemoveMod()
        {
            source.GetWAt(at).RemoveModifier(modType, name);
        }
    }
}