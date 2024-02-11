using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CodeTao
{
    /// <summary>
    /// 装填子弹的执行器。
    /// </summary>
    public class ReloadBehaviour : BehaviourNode
    {
        public BindableStat shotsToReload = new BindableStat(0);
        public BindableStat reloadTime = new BindableStat(0);
        List<WAtModder> modders = new List<WAtModder>();
        int shotsLeft = 0;

        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            modders = GetComponents<WAtModder>().ToList();
        }

        protected override IEnumerator Executing()
        {
            modders.ForEach(m => m.AddMod());
            if (shotsLeft < shotsToReload)
            {
                shotsLeft++;
            }
            else
            {
                shotsLeft = 0;
                modders.ForEach(m => m.RemoveMod());
                yield return new WaitForSeconds(reloadTime);
            }
        }
    }
}