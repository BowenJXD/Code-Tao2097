using QFramework;

namespace CodeTao
{
    public class CycleOfLifeAndDeath : Blessing
    {
        /// <summary>
        /// 0 - 100
        /// </summary>
        public BindableStat chance = new BindableStat(100);
        public BindableStat healingValue = new BindableStat(1);
        public BindableStat cooldown = new BindableStat(0.1f);
        public ElementType elementType = ElementType.Wood;
        public bool isInCD = false;

        public override void OnAdd()
        {
            base.OnAdd();
            EnemyManager.Instance.AddOnSpawnAction(OnSpawn);
        }

        void OnSpawn(Enemy enemy)
        {
            enemy.onDeinit += () =>
            {
                if (isInCD)
                {
                    return;
                }
                BuffOwner buffOwner = enemy.GetComp<BuffOwner>();
                if (buffOwner && buffOwner.FindAll(buff => buff.elementType == elementType).Count > 0)
                {
                    if (RandomUtil.Rand100(chance))
                    {
                        Player.Instance.GetComp<Defencer>().TakeHealing(healingValue);
                        if (cooldown > 0){
                            isInCD = true;
                            ActionKit.Delay(cooldown, () => isInCD = false).Start(this);
                        }
                    }
                }
            };
        }
    }
}