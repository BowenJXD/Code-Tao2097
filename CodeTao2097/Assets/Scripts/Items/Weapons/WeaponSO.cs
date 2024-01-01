using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    [CreateAssetMenu(menuName = "Items/Weapons", fileName = "New Weapon")]
    public class WeaponSO : ItemSO
    {
        public ElementType elementType;
        
        public string buffToApply;
        public int buffHitRate;
        
        public float damage;
        public int amount;
        public float duration;
        public float speed;
        public float cooldown;
        public float area;
        
        public int shotsToReload;
        public float reloadTime;
        public float attackRange;
        public int knockback;
        
        public List<WeaponUpgradeMod> upgradeMods;
    }
}