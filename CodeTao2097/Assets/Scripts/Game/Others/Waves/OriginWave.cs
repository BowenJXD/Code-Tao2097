using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class OriginWave : Wave
    {
        protected override void Attack(Collider2D col)
        {
            base.Attack(col);
            TagOwner tagOwner = col.GetComponentFromUnit<TagOwner>();
            if (tagOwner && tagOwner.HasTag(Tag.Resonating))
            {
                ResonanceWave wave = ResonanceWaveManager.Instance.Get();
                wave.Parent(tagOwner.transform).Position(tagOwner.transform.position);
                wave.Init();
            }
        }
    }
}