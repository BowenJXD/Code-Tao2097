using System;
using System.Collections.Generic;
using QFramework;

namespace CodeTao
{
    /// <summary>
    /// 属性管理器组件，用于管理单位的各种属性。
    /// </summary>
    public class AttributeController : ViewController
    {
        public Dictionary<EAAt, ModifierGroup> artefactModGroups = new Dictionary<EAAt, ModifierGroup>();
        public Dictionary<EWAt, ModifierGroup> weaponModGroups = new Dictionary<EWAt, ModifierGroup>();

        public void AddArtefactModifier(EAAt at, float value, EModifierType modifierType, string modName,
            RepetitionBehavior repetitionBehavior = RepetitionBehavior.Return)
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

        public Action<EAAt, ModifierGroup> onAddAAtModGroup;
        
        public bool AddItemModGroup(EAAt at)
        {
            if (artefactModGroups.ContainsKey(at))
            {
                return false;
            }
            else
            {
                artefactModGroups.Add(at, new ModifierGroup());
                onAddAAtModGroup?.Invoke(at, artefactModGroups[at]);
                return true;
            }
        }

        public void AddWeaponModifier(EWAt at, float value, EModifierType modifierType, string modName = "inventory",
            RepetitionBehavior repetitionBehavior = RepetitionBehavior.Return)
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
        
        public Action<EWAt, ModifierGroup> onAddWAtModGroup;
        
        public bool AddWeaponModGroup(EWAt at)
        {
            if (weaponModGroups.ContainsKey(at))
            {
                return false;
            }
            else
            {
                weaponModGroups.Add(at, new ModifierGroup());
                onAddWAtModGroup?.Invoke(at, weaponModGroups[at]);
                return true;
            }
        }

        private void OnDisable()
        {
            foreach (var modGroup in artefactModGroups)
            {
                modGroup.Value.Clear();
            }
            artefactModGroups.Clear();
            foreach (var modGroup in weaponModGroups)
            {
                modGroup.Value.Clear();
            }
            weaponModGroups.Clear();
        }
    }
}