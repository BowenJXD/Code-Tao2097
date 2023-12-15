using System;
using CodeTao;
using QFramework;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Buffs
{
    public class Buff : Content<Buff>
    {
        public LoopTask buffLoop;
    }
    
}