namespace CodeTao
{
    public class InfernalFire : Blessing
    {
        public override void Init()
        {
            base.Init();
            UnitManager.Instance.AddOnGetAction<GroundEffect>(OnSpawn);
        }
        
        void OnSpawn(GroundEffect unit)
        {
            if (unit.attackWhenEntering == EAttackTarget.None) unit.attackWhenEntering.Value = EAttackTarget.One;
            if (unit.attackWhenExiting == EAttackTarget.None) unit.attackWhenExiting.Value = EAttackTarget.One;
        }
    }
}