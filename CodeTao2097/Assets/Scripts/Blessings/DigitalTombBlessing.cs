using QFramework;

namespace CodeTao
{
    public class DigitalTombBlessing : Blessing
    {
        /// <summary>
        /// 0 - 100
        /// </summary>
        public BindableStat chance = new BindableStat(10f);

        public override void OnAdd()
        {
            base.OnAdd();
            EnemyManager.Instance.AddOnSpawnAction(OnSpawn);
        }

        void OnSpawn(Enemy enemy)
        {
            enemy.onDeinit += () =>
            {
                if (RandomUtil.Rand100(chance))
                {
                    DigitalTombGenerator.Instance.Get()?.Position(enemy.transform.position).Init();
                }
            };
        }
    }
    
}