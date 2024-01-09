using System;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 用于设置一个artefact升级可以为单位带来的属性变化
    /// </summary>
    [Serializable]
    public class ArtefactUpgradeMod : UpgradeMod
    {
        public EAAt attribute;

        public override string GetAttribute()
        {
            return attribute.ToString();
        }
    }
}