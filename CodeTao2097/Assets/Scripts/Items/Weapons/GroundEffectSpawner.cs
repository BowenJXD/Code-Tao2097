using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 生成领域的武器
    /// </summary>
    public partial class GroundEffectSpawner : SpawnerWeapon<GroundEffect>
    {
        [BoxGroup("Ground Effect Spawner")]
        public List<Vector2> spawnPoints = new List<Vector2>();
        private int _currentSpawnPointIndex = 0;
        [BoxGroup("Ground Effect Spawner")]
        public EAimWay aimWay = EAimWay.Random;
        [BoxGroup("Ground Effect Spawner")]
        public float spawnPointMaxOffset = 1;
        [BoxGroup("Ground Effect Spawner")]
        public bool rootToOwner = false;
        
        private MoveController _ownerMoveController;

        public override void OnAdd()
        {
            base.OnAdd();
            _ownerMoveController = Container.GetComp<MoveController>();
        }

        public override GroundEffect SpawnUnit(Vector2 globalPos, Vector2 localPos)
        {
            GroundEffect unit = base.SpawnUnit(globalPos, localPos);
            unit.attackInterval.Value = ats[EWAt.Cooldown].Value;
            unit.lifeTime.Value = ats[EWAt.Duration].Value;
            unit.Parent(rootToOwner ? transform : GroundEffectManager.Instance.transform)
                .LocalScale(new Vector3(ats[EWAt.Area], ats[EWAt.Area]))
                .SetWeapon(this);
            return unit;
        }

        public override Vector2 GetLocalSpawnPoint(Vector2 basePoint, int spawnIndex)
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
                    // find the nearest spawn point in spawnPoints regarding to angle
                    if (spawnPoints.Count > 0)
                    {
                        result = spawnPoints.OrderBy(v => Vector2.Angle(result, v)).First();
                    }
                    else
                    {
                        result = _ownerMoveController.LastNonZeroDirection.Value * attackRange;
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
                    // find the nearest spawn point in spawnPoints regarding to distance
                    if (spawnPoints.Count > 0)
                    {
                        result = spawnPoints.OrderBy(v => Vector2.Distance(result, v)).First();
                    }
                    else
                    {
                        result = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
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
            result += randomOffset;
            return result;
        }
    }
}