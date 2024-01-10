using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 鸣潮Origin Wave
    /// 生成Originwave冲击波，冲击波从自身出发，呈圆形向外扩散。
    /// </summary>
    public class OriginWaveSpawner : SpawnerWeapon<OriginWave>
    {
        public override void Init()
        {
            base.Init();
            ats[EWAt.Amount].SetMaxValue(1);
        }

        public override OriginWave SpawnUnit(Vector2 localPos)
        {
            OriginWave unit = base.SpawnUnit(localPos);
            unit.lifeTime.Value = ats[EWAt.Duration].Value;
            unit.range.Value = ats[EWAt.Area].Value;
            unit.Parent(Global.Instance.transform);
            unit.SetWeapon(this);
            return unit;
        }

        public override Vector2 GetLocalSpawnPoint(Vector2 basePoint, int spawnIndex)
        {
            return Vector2.zero;
        }
    }
}