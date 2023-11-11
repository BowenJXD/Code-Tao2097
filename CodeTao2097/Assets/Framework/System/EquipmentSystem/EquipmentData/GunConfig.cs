namespace QFramework.EquipmentSystem
{
    [System.Serializable]
    public class GunConfig : EquipConfig, IEquipConfig<GunInfo>
    {
        GunInfo IEquipConfig<GunInfo>.Create()
        {
            return new GunInfo();
        }
    }
}