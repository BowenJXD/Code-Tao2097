using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 共振波
    /// </summary>
    public class ResonanceWave : Wave, IWeaponDerivative
    {
        public override void SetWeapon(Weapon newWeapon, Damager newDamager)
        {
            base.SetWeapon(newWeapon, newDamager);
            damager.AddDamageTag(DamageTag.Resonance);
        }
    }
}