using System.Linq;
using QFramework;

namespace CodeTao
{
    public class TurbulenceBlessing : Blessing
    {
        MoveController moveController;
        public float totalDistance = 2;
        public UnitController unit;
        private Damager damager;

        public override void Init()
        {
            base.Init();
            moveController = Container.GetComp<MoveController>();
            AttributeController atController = Container.GetComp<AttributeController>();
            damager = this.GetComponentInDescendants<Damager>();
            unit = this.GetComponentInDescendants<UnitController>(true);
            atController.As<IWAtSource>().Transmit(GetComponentsInChildren<IWAtReceiver>(true));
            UnitManager.Instance.Register(unit);
            
            moveController.RegisterDistanceEvent(totalDistance, DistanceEvent);
        }
        
        void DistanceEvent()
        {
            UnitController newUnit = UnitManager.Instance.Get(unit).Position(moveController.transform.position);
            newUnit.Link.AddComponent(damager);
            newUnit.Init();
            moveController.RegisterDistanceEvent(totalDistance, DistanceEvent);
        }
    }
}