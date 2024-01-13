using UnityEngine;

namespace CodeTao
{
    public interface IWeaponDerivative
    {
        public Weapon weapon { get; set; }
        public void SetWeapon(Weapon newWeapon, Damager newDamager);
        
        public void InitSpawn(Vector3 globalPos){}
    }
}