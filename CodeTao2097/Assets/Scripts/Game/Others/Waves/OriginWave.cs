using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 鸣潮武器生成的单位，会在触碰到有共振标签的单位时生成共振波。
    /// </summary>
    public class OriginWave : Wave
    {
        protected override void Attack(Collider2D col)
        {
            base.Attack(col);
            TagOwner tagOwner = col.GetComp<TagOwner>();
            if (tagOwner && tagOwner.HasTag(Tag.Resonating))
            {
                ResonanceWaveManager.Instance.Get().Position(tagOwner.transform.position).Init();
            }
        }
    }
}