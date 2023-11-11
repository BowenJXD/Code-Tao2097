namespace QFramework.EquipmentSystem
{
    [System.Serializable]
    public class SwordConfig : EquipConfig, IEquipConfig<SwordInfo>
    {
        SwordInfo IEquipConfig<SwordInfo>.Create()
        {
            return new SwordInfo();
        }
    }
}