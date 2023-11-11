using UnityEngine;

namespace QFramework
{
    [CreateAssetMenu(fileName = "New_BagItemConfig", menuName = "Data/SO/BagConfig")]
    public class SO_BagItemDatas : ScriptableObject
    {
        public BagItemConfig[] configs;
    }
}