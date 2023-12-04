using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    public class GroundEffectSpawner : SpawnerWeapon<GroundEffect>
    {
        public List<Vector2> spawnPoints = new List<Vector2>();
        private int _currentSpawnPointIndex = 0;
        public EAimWay aimWay = EAimWay.Random;
        
        public float spawnPointMaxOffset = 1;

        public override Vector2 GetSpawnPoint(int spawnIndex)
        {
            Vector2 result = Util.GetRandomNormalizedVector();
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
                            result = target.transform.position;
                        }
                        break;
                    case EAimWay.Owner:
                        result = Vector2.zero;
                        break;
                    case EAimWay.Random:
                        float r2 = Global.Instance.Random.Next(attackRange);
                        result = Util.GetRandomNormalizedVector() * r2;
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
    }
}