using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public partial class GroundEffectSpawner : SpawnerWeapon<GroundEffect>
    {
        public List<Vector2> spawnPoints = new List<Vector2>();
        private int _currentSpawnPointIndex = 0;
        public EAimWay aimWay = EAimWay.Random;
        
        public float spawnPointMaxOffset = 1;
        public bool rootToOwner = false;
        
        private MoveController _ownerMoveController;

        private void Awake()
        {
            AddAfter += content =>
            {
                _ownerMoveController = ComponentUtil.GetComponentFromUnit<MoveController>(Container);
            };
        }

        public override GroundEffect SpawnUnit(Vector2 spawnPosition)
        {
            GroundEffect unit = base.SpawnUnit(spawnPosition);
            unit.attackInterval.Value = ats[EWAt.Cooldown].Value;
            unit.lifeTime.Value = ats[EWAt.Duration].Value;
            unit.Parent(rootToOwner? transform : GroundEffectManager.Instance.transform)
                .LocalScale(new Vector3(ats[EWAt.Area], ats[EWAt.Area]))
                .Init(this);
            return unit;
        }

        public override Vector2 GetSpawnPoint(int spawnIndex)
        {
            Vector2 result = RandomUtil.GetRandomScreenPosition();
            switch (aimWay)
            {
                case EAimWay.AutoTargeting:
                    var targets = GetTargets();
                    if (targets.Count > 0)
                    {
                        int r1 = Global.Instance.Random.Next(targets.Count);
                        Defencer target = targets[r1];
                        result = target.transform.position - transform.position;
                        
                        // find the nearest spawn point in spawnPoints regarding to distance
                        if (spawnPoints.Count > 0)
                        {
                            result = spawnPoints.OrderBy(v => Vector2.Distance(result, v)).First();
                        }
                    }
                    break;
                case EAimWay.Owner:
                    result = _ownerMoveController.LastNonZeroDirection.Value * attackRange;
                    
                    // find the nearest spawn point in spawnPoints regarding to angle
                    if (spawnPoints.Count > 0)
                    {
                        result = spawnPoints.OrderBy(v => Vector2.Angle(result, v)).First();
                    }
                    break;
                case EAimWay.Random:
                    if (spawnPoints.Count > 0)
                    {
                        int r2 = Global.Instance.Random.Next(spawnPoints.Count);
                        result = spawnPoints[r2];
                    }
                    break;
                case EAimWay.Cursor:
                    result = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
                    
                    // find the nearest spawn point in spawnPoints regarding to distance
                    if (spawnPoints.Count > 0)
                    {
                        result = spawnPoints.OrderBy(v => Vector2.Distance(result, v)).First();
                    }
                    break;
                case EAimWay.Sequential:
                    if (spawnPoints.Count > 0)
                    {
                        result = spawnPoints[_currentSpawnPointIndex];
                        _currentSpawnPointIndex += 1;
                        _currentSpawnPointIndex %= spawnPoints.Count;
                    }

                    break;
            }
            Vector2 randomOffset = RandomUtil.GetRandomNormalizedVector() * spawnPointMaxOffset * Global.Instance.Random.Next(1);
            result = result + randomOffset;
            return result;
        }
    }
}