using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    public partial class GroundEffectSpawner : SpawnerWeapon<GroundEffect>
    {
        public List<Vector2> spawnPoints = new List<Vector2>();
        private int _currentSpawnPointIndex = 0;
        public EAimWay aimWay = EAimWay.Random;
        
        public float spawnPointMaxOffset = 1;

        public override GroundEffect SpawnUnit(Vector2 spawnPosition)
        {
            GroundEffect unit = base.SpawnUnit(spawnPosition);
            unit.transform.parent = GroundEffectManager.Instance.transform;
            unit.attackInterval.Value = ats[EWAts.Cooldown].Value;
            unit.lifeTime.Value = ats[EWAts.Duration].Value;
            unit.Init(this);
            return unit;
        }

        public override Vector2 GetSpawnPoint(int spawnIndex)
        {
            float r2 = Global.Instance.Random.Next(ats[EWAts.Area]);
            Vector2 result = Util.GetRandomNormalizedVector() * r2;
            if (spawnPoints.Count > 0)
            {
                result = spawnPoints[_currentSpawnPointIndex];
                _currentSpawnPointIndex += 1;
                _currentSpawnPointIndex %= spawnPoints.Count;
            }
            else
            {
                switch (aimWay)
                {
                    case EAimWay.AutoTargeting:
                        var targets = GetTargets();
                        if (targets.Count > 0)
                        {
                            int r1 = Global.Instance.Random.Next(targets.Count);
                            Defencer target = targets[r1];
                            result = target.transform.position - transform.position;
                        }
                        break;
                    case EAimWay.Owner:
                        result = Vector2.zero;
                        break;
                    case EAimWay.Random:
                        break;
                    case EAimWay.Cursor:
                        result = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
                        break;
                }
            }
            Vector2 randomOffset = Util.GetRandomNormalizedVector() * spawnPointMaxOffset * Global.Instance.Random.Next(1);
            result = result + randomOffset;
            return result;
        }

        public override void Upgrade(int lvlIncrement = 1)
        {
            base.Upgrade(lvlIncrement);
            switch (LVL.Value)
            {
                default:
                    ats[EWAts.Cooldown].AddModifier($"Level{LVL.Value}", 1 - 0.1f, EModifierType.Multiplicative, ERepetitionBehavior.NewStack);
                    break;
            }
        }

        public override string GetDescription()
        {
            string result = $"{GetType()}'s attack interval - 10%";
            return result;
        }
    }
}