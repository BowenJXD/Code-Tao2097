using UnityEngine;

namespace CodeTao
{
    public abstract class ItemSO : ScriptableObject
    {
        public string id;
        public string itemName;
        public string description;
        public string icon;
    }
}