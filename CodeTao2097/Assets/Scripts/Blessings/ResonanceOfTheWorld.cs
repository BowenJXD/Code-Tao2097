using QFramework;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 万物共振（金）
    /// 每隔一段时间引发一次共振，在每个参与共振的个体周围造成1次范围伤害（受攻击力影响）。
    /// 通过physics获取范围内的所有个体，然后判断是否有共振标签，如果有，就在其周围产生一个共振波。
    /// Resonance of the world (gold)
    /// Every once in a while, a resonance is triggered, causing damage to the area around each participating individual (affected by atk).
    /// </summary>
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
                TagOwner tagOwner = col.GetComp<TagOwner>();
                if (tagOwner && tagOwner.HasTag(Tag.Resonating))
                {
                    ResonanceWave wave = ResonanceWaveGenerator.Instance.Get();
                    wave.Position(tagOwner.transform.position);
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