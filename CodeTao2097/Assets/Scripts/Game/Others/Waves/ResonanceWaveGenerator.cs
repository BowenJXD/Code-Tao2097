﻿using System;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 共振波管理器，会覆盖共振波的DMG，并基于玩家的ATK来设置DMG。
    /// </summary>
    public class ResonanceWaveGenerator : UnitGenerator<ResonanceWave, ResonanceWaveGenerator>
    {
        public static Damager damager;

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            damager = this.GetComponentInDescendants<Damager>();
            Attacker attacker = Player.Instance.GetComp<Attacker>();
            if (attacker)
            {
                attacker.ATK.RegisterWithInitValue(value =>
                {
                    damager.DMG.AddModifier(value, EModifierType.Multiplicative, "ATK", RepetitionBehavior.Overwrite, true);
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
        }

        public override ResonanceWave Get()
        {
            ResonanceWave obj = base.Get();
            obj.SetDamager(damager);
            return obj;
        }
    }
}