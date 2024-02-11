namespace CodeTao
{
    /// <summary>
    /// 每走一段路发动一次冲击。
    /// </summary>
    public class MoveFireBehaviour : WeaponBehaviour
    {
        private MoveController moveController;
        public float totalDistance;
        
        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            UnNext();
            moveController = weapon.Container.GetComp<MoveController>();
            moveController.RegisterDistanceEvent(totalDistance, Next);
        }
    }
}