using System;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 用于设置一个weapon升级的属性变化
    /// </summary>
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