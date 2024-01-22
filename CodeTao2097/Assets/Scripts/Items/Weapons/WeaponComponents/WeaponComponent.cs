using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 武器组件。
    /// </summary>
    public class WeaponComponent : MonoBehaviour
    {
        [HideInInspector] public Weapon weapon;
        
        public virtual void Init(Weapon newWeapon)
        {
            weapon = newWeapon;
        }
    }
}