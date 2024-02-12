using QFramework;

namespace CodeTao
{
    public class SpawnUnitBehaviour : BehaviourNode
    {
        public UnitController unitPrefab;
        public bool rootToParent = false;
        private Weapon weapon;
        private Damager damager;

        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            if (!unitPrefab) unitPrefab = GetComponentInChildren<UnitController>(true);
            if (!damager) damager = GetComponentInChildren<Damager>();
            UnitManager.Instance.Register(unitPrefab, rootToParent? transform : null);
            if (!weapon)
            {
                weapon = this.GetComponentInAncestors<Weapon>();
            }
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            UnitController unit = UnitManager.Instance.Get(unitPrefab);
            unit.Position(transform.position);

            if (unit is IWeaponDerivative weaponDerivative){
                weaponDerivative.SetWeapon(weapon, damager);
                weaponDerivative.InitSpawn(transform.position);
            }
            unit.Init();
        }
    }
}