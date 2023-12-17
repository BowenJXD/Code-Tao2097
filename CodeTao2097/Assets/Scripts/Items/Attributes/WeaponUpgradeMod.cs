using System;
using UnityEngine;

namespace CodeTao
{
    [Serializable]
    public class WeaponUpgradeMod : UpgradeMod
    {
        public EWAt attribute;

        public override string GetAttribute()
        {
            return attribute.ToString();
        }
    }
}