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
        
        protected LoopTask loopTask;

        public override void OnAdd()
        {
            base.OnAdd();

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
                    ResonanceWave wave = ResonanceWaveManager.Instance.Get();
                    wave.Parent(tagOwner.transform).Position(tagOwner.transform.position);
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