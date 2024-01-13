using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeTao
{
    public class SpatialSelector : WeaponSelector
    {
        private int _currentSpawnPointIndex = 0;
        public EAimWay aimWay = EAimWay.Random;
        public float spawnPointMaxOffset = 1;
        public List<Vector2> spawnPoints = new List<Vector2>();
        
        private MoveController _ownerMoveController;

        public override void Init(Weapon newWeapon)
        {
            base.Init(newWeapon);
            _ownerMoveController = newWeapon.Container.GetComp<MoveController>();
        }

        public override List<Vector3> GetGlobalPositions()
        {
            int amount = (int)weapon.amount.Value;
            List<Vector3> results = new List<Vector3>();
            for (int i = 0; i < amount; i++)
            {
                Vector3 result = transform.position + GetLocalPosition(i);
                results.Add(result);
            }

            return results;
        }

        public Vector3 GetLocalPosition(int spawnIndex)
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
                        result = _ownerMoveController.LastNonZeroDirection.Value * weapon.attackRange;
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