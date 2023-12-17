using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public partial class Inventory : Container<Item>
    {
        public Dictionary<EAAt, ModifierGroup> itemModGroups = new Dictionary<EAAt, ModifierGroup>();

        private Attacker _attacker;
        private MoveController _moveController;
        private Defencer _defencer;
        private ExpController _expController;
        
        public List<Weapon> weaponInventory = new List<Weapon>();
        public Dictionary<EWAt, ModifierGroup> weaponModGroups = new Dictionary<EWAt, ModifierGroup>();

        private void Start()
        {
            _attacker = ComponentUtil.GetComponentFromUnit<Attacker>(this);
            _moveController = ComponentUtil.GetComponentFromUnit<MoveController>(this);
            _defencer = ComponentUtil.GetComponentFromUnit<Defencer>(this);
            _expController = ComponentUtil.GetComponentFromUnit<ExpController>(this);
            
            List<Item> items = ComponentUtil.GetComponentsInDescendants<Item>(this);
            foreach (var item in items)
            {
                item.AddToContainer(this);
                item.Upgrade();
            }
        }

        public override void ProcessAddedContent(Content<Item> content)
        {
            Item item = content as Item;
            if (item == null)
            {
                return;
            }
            
            Transform parent = transform;
            
            if (item.GetType() == typeof(Weapon) || item.CompareTag("Weapon"))
            {
                parent = Weapons;
                weaponInventory.Add(item as Weapon);
            }
            else if (item.GetType() == typeof(Item) || item.CompareTag("Item"))
            {
                parent = Items;
            }
            
            if (item.transform.parent != parent)
            {
                item.transform.SetParent(parent, false);
            }
        }
        
        public bool ContainsItem(Item item)
        {
            return Contents.Contains(item);
        }
        
        public void AddArtefactModifier(EAAt at, float value, EModifierType modifierType, string modName = "inventory",
            ERepetitionBehavior repetitionBehavior = ERepetitionBehavior.Return)
        {
            if (itemModGroups.ContainsKey(at))
            {
                itemModGroups[at].AddModifier(value, modifierType, modName);
            }
            else
            {
                AddItemModGroup(at);
                itemModGroups[at].AddModifier(value, modifierType, modName);
            }
        }

        public bool AddItemModGroup(EAAt at)
        {
            if (itemModGroups.ContainsKey(at))
            {
                return false;
            }
            else
            {
                itemModGroups.Add(at, new ModifierGroup());
                BindToItemStat(at);
                return true;
            }
        }

        public void BindToItemStat(EAAt at)
        {
            if (!itemModGroups.ContainsKey(at)) return;
            
            switch (at)
            {
                case EAAt.ATK:
                    _attacker.ATK.AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.CritRate:
                    _attacker.CritRate.AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.CritDamage:
                    _attacker.CritDamage.AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.AllElementBON:
                    _attacker.ElementBonuses[ElementType.All].AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.MetalElementBON:
                    _attacker.ElementBonuses[ElementType.Metal].AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.WoodElementBON:
                    _attacker.ElementBonuses[ElementType.Wood].AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.WaterElementBON:
                    _attacker.ElementBonuses[ElementType.Water].AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.FireElementBON:
                    _attacker.ElementBonuses[ElementType.Fire].AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.EarthElementBON:
                    _attacker.ElementBonuses[ElementType.Earth].AddModifierGroup(itemModGroups[at]);
                    break;
                
                case EAAt.DEF:
                    _defencer.DEF.AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.MaxHP:
                    _defencer.MaxHP.AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.AllElementRES:
                    _defencer.ElementResistances[ElementType.All].AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.MetalElementRES:
                    _defencer.ElementResistances[ElementType.Metal].AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.WoodElementRES:
                    _defencer.ElementResistances[ElementType.Wood].AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.WaterElementRES:
                    _defencer.ElementResistances[ElementType.Water].AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.FireElementRES:
                    _defencer.ElementResistances[ElementType.Fire].AddModifierGroup(itemModGroups[at]);
                    break;
                case EAAt.EarthElementRES:
                    _defencer.ElementResistances[ElementType.Earth].AddModifierGroup(itemModGroups[at]);
                    break;
                
                case EAAt.SPD:
                    _moveController.SPD.AddModifierGroup(itemModGroups[at]);
                    break;
                
                case EAAt.EXPBonus:
                    _expController.EXPRate.AddModifierGroup(itemModGroups[at]);
                    break;
                
                default:
                    break;
            }
        }
        
        public void AddWeaponModifier(EWAt at, float value, EModifierType modifierType, string modName = "inventory",
            ERepetitionBehavior repetitionBehavior = ERepetitionBehavior.Return)
        {
            if (weaponModGroups.ContainsKey(at))
            {
                weaponModGroups[at].AddModifier(value, modifierType, modName);
            }
            else
            {
                AddWeaponModGroup(at);
                weaponModGroups[at].AddModifier(value, modifierType, modName);
            }
        }
        
        public bool AddWeaponModGroup(EWAt at)
        {
            if (weaponModGroups.ContainsKey(at))
            {
                return false;
            }
            else
            {
                weaponModGroups.Add(at, new ModifierGroup());
                BindToWeaponStat(at);
                return true;
            }
        }

        public void BindToWeaponStat(EWAt at)
        {
            if (!weaponModGroups.ContainsKey(at)) return;

            foreach (var weapon in weaponInventory)
            {
                weapon.ats[at].AddModifierGroup(weaponModGroups[at]);
            }
        }
    }
}