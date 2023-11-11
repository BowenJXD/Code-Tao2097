using UnityEngine;

namespace QFramework
{
    [CreateAssetMenu(fileName = "New EquipTypeConfig", menuName = "Data/SO/EquipTypeConfig")]
    public class SO_EquipTypeConfig : ScriptableObject
    {
        public E_EquipType types;
    }
}