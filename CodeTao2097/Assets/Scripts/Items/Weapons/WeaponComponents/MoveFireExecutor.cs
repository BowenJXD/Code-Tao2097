namespace CodeTao
{
    /// <summary>
    /// 每走一段路发动一次冲击。
    /// </summary>
    public class MoveFireExecutor : WeaponExecutor
    {
        private MoveController moveController;
        public float totalDistance;
        
        public override void Init(Weapon newWeapon)
        {
            base.Init(newWeapon);
            moveController = weapon.Container.GetComp<MoveController>();
            moveController.RegisterDistanceEvent(totalDistance, Next);
        }
    }
}