namespace QFramework.EquipmentSystem
{
    public class EquipmentFactorySystem : AbstractEquipmentFactorySystem
    {
        protected override EquipInfo CreateRules(E_EquipType type, EquipConfig equip)
        {
            switch (type)
            {
				case E_EquipType.Gun: return CreateEquip<GunInfo>(equip);
				case E_EquipType.Sword: return CreateEquip<SwordInfo>(equip);
            }
            return null;
        }
    }
}