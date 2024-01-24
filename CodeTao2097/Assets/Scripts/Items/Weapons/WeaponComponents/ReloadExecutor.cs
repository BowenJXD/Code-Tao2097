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
    public class ReloadExecutor : WeaponExecutor
    {
        public BindableStat shotsToReload = new BindableStat(0);
        public BindableStat reloadTime = new BindableStat(0);
        List<WAtModder> modders = new List<WAtModder>();
        int shotsLeft = 0;

        public override void Init(Weapon newWeapon)
        {
            base.Init(newWeapon);
            modders = GetComponents<WAtModder>().ToList();
        }

        public override IEnumerator ExecuteCoroutine(List<Vector3> globalPositions)
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
            }

            yield return base.ExecuteCoroutine(globalPositions);
        }
    }
}