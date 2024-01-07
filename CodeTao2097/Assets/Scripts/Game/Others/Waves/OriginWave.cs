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
                ResonanceWaveManager.Instance.Get().Position(tagOwner.transform.position).Init();
            }
        }
    }
}