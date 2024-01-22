using System;
using System.Collections.Generic;
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
    }
}