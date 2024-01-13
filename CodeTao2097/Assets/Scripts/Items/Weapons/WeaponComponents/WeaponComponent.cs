using UnityEngine;

namespace CodeTao
{
    public class WeaponComponent : MonoBehaviour
    {
        [HideInInspector] public Weapon weapon;
        
        public virtual void Init(Weapon newWeapon)
        {
            weapon = newWeapon;
        }
    }
}