using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;

namespace CodeTao
{
    public class EnemyManager : MonoSingleton<EnemyManager>
    {
        public List<EnemyGenerator> enemyGenerators = new List<EnemyGenerator>();

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            enemyGenerators.AddRange(this.GetComponentsInDescendants<EnemyGenerator>());
        }

        public void AddOnSpawnAction(Action<Enemy> action)
        {
            foreach (var enemyGenerator in enemyGenerators)
            {
                enemyGenerator.onSpawn += action;
            }
        }
        
        public Enemy GetRandom()
        {
            int totalCount = enemyGenerators.Sum(enemyGenerator => enemyGenerator.activeEnemies.Count);
            int index = RandomUtil.Rand(totalCount);
            foreach (var enemyGenerator in enemyGenerators)
            {
                if (index < enemyGenerator.activeEnemies.Count)
                {
                    return enemyGenerator.activeEnemies[index];
                }
                index -= enemyGenerator.activeEnemies.Count;
            }
            return null;
        }
    }
}