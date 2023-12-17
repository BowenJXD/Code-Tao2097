using System;
using UnityEngine;

namespace CodeTao
{
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