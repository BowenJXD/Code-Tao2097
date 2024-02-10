using QFramework;

namespace CodeTao
{
    public class SpawnBehaviour : UnitBehaviour
    {
        public UnitController unitPrefab;
        public bool rootToParent = true;
        private Weapon weapon;
        private Damager damager;

        private void OnEnable()
        {
            if (!unitPrefab) unitPrefab = GetComponentInChildren<UnitController>(true);
            if (!damager) damager = GetComponentInChildren<Damager>();
            UnitManager.Instance.Register(unitPrefab, rootToParent? transform : null);
            if (!weapon){
                if (Unit is IWeaponDerivative weaponDerivative)
                {
                    weapon = weaponDerivative.weapon;
                }
            }
        }

        protected void Spawn()
        {
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