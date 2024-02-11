using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 空间选择器，用于生成攻击的位置信息。
    /// </summary>
    public class SpatialSelector : WeaponSelector
    {
        private int _currentSpawnPointIndex = 0;
        public EAimWay aimWay = EAimWay.Random;
        public float spawnPointMaxOffset = 1;
        public List<Vector2> spawnPoints = new List<Vector2>();
        
        private MoveController _ownerMoveController;

        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            _ownerMoveController = weapon.Container.GetComp<MoveController>();
        }
        
        public override Vector3 GetLocalPosition(int spawnIndex)
        {
            Vector2 result = RandomUtil.GetRandomScreenPosition();
            switch (aimWay)
            {
                case EAimWay.AutoTargeting:
                    var targets = weapon.GetTargets();
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
                        result = _ownerMoveController.lastNonZeroDirection.Value * weapon.attackRange;
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