using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class ResonanceWaveManager : UnitManager<ResonanceWave>
    {
        public static Damager damager;

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            damager = this.GetComponentInDescendants<Damager>();
            Attacker attacker = Player.Instance.GetComponentFromUnit<Attacker>();
            if (attacker)
            {
                attacker.ATK.RegisterWithInitValue(value =>
                {
                    damager.DMG.AddModifier(value, EModifierType.MultiAdd, "ATK", ERepetitionBehavior.Overwrite);
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