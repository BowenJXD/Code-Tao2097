using System;
using System.Collections.Generic;
using QFramework;

namespace CodeTao
{
    public class AttributeController : ViewController
    {
        public Dictionary<EAAt, BindableStat> artefactModGroups = new Dictionary<EAAt, BindableStat>();
        public Dictionary<EWAt, BindableStat> weaponModGroups = new Dictionary<EWAt, BindableStat>();

        public void AddArtefactModifier(EAAt at, float value, EModifierType modifierType, string modName,
            ERepetitionBehavior repetitionBehavior = ERepetitionBehavior.Return)
        {
            if (artefactModGroups.ContainsKey(at))
            {
                artefactModGroups[at].AddModifier(value, modifierType, modName, repetitionBehavior);
            }
            else
            {
                AddItemModGroup(at);
                artefactModGroups[at].AddModifier(value, modifierType, modName, repetitionBehavior);
            }
        }
        
        public void RemoveArtefactModifier(EAAt at, EModifierType modifierType, string modName)
        {
            if (artefactModGroups.ContainsKey(at))
            {
                artefactModGroups[at].RemoveModifier(modifierType, modName);
            }
        }

        public Action<EAAt, BindableStat> onAddAAtModGroup;
        
        public bool AddItemModGroup(EAAt at)
        {
            if (artefactModGroups.ContainsKey(at))
            {
                return false;
            }
            else
            {
                artefactModGroups.Add(at, new BindableStat());
                onAddAAtModGroup?.Invoke(at, artefactModGroups[at]);
                return true;
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
        
        public Action<EWAt, BindableStat> onAddWAtModGroup;
        
        public bool AddWeaponModGroup(EWAt at)
        {
            if (weaponModGroups.ContainsKey(at))
            {
                return false;
            }
            else
            {
                weaponModGroups.Add(at, new BindableStat());
                onAddWAtModGroup?.Invoke(at, weaponModGroups[at]);
                return true;
            }
        }
    }
}