using QFramework;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace CodeTao
{
    public class ResonanceOfTheWorld : Blessing
    {
        public BindableStat interval = new BindableStat(4f);
        public BindableStat range = new BindableStat(10);
        public BindableStat DMG = new BindableStat(10);
        
        protected Damager damager;
        
        protected LoopTask loopTask;
        
        protected UnitPool<ResonanceWave> wavePool;
        public ResonanceWave wavePrefab;

        public override void OnAdd()
        {
            base.OnAdd();

            damager = this.GetComponentInDescendants<Damager>();
            damager.DMG = DMG;

            Attacker attacker = this.GetComponentFromUnit<Attacker>();
            if (attacker)
            {
                attacker.ATK.RegisterWithInitValue(value =>
                {
                    DMG.AddModifier(value, EModifierType.MultiAdd, "ATK", ERepetitionBehavior.Overwrite);
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
            
            wavePrefab = this.GetComponentInDescendants<ResonanceWave>(true);
            wavePool = new UnitPool<ResonanceWave>(wavePrefab, ResonanceWaveManager.Instance.transform);
            
            loopTask = new LoopTask(this, interval.Value, Trigger);
            loopTask.Start();
        }

        public void Trigger()
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, range);
            foreach (Collider2D col in cols)
            {
                TagOwner tagOwner = col.GetComponentFromUnit<TagOwner>();
                if (tagOwner && tagOwner.HasTag(Tag.Resonating))
                {
                    ResonanceWave wave = wavePool.Get().Parent(tagOwner.transform).Position(tagOwner.transform.position);
                    wave.SetDamager(damager);
                    wave.onDeinit = () =>
                    {
                        wavePool.Release(wave);
                    };
                    wave.Init();
                }
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            loopTask.Pause();
        }
    }
}