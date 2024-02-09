using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using Sirenix.OdinInspector;

namespace CodeTao
{
    /// <summary>
    /// 属性管理器组件，用于管理单位的各种属性。
    /// </summary>
    public class AttributeController : UnitComponent, IAAtSource, IWAtSource
    {
        // initialise the dictionary with all the enum values
        public SerializableDictionary<EAAt, BindableStat> aStats = new();
        public SerializableDictionary<EWAt, BindableStat> wStats = new();
        protected Inventory inventory;

        private void OnEnable()
        {
            if (!inventory) inventory = GetComp<Inventory>();
            List<IAAtReceiver> aAtReceivers = Unit.GetComponentsInDescendants<IAAtReceiver>(true).ToList(); 
            this.As<IAAtSource>().Transmit(aAtReceivers);
        }

        public void AddArtefactModifier(EAAt at, float value, EModifierType modifierType, string modName,
            RepetitionBehavior repetitionBehavior = RepetitionBehavior.Return)
        {
            if (!aStats.ContainsKey(at))
            {
                AddArtefactModGroup(at);
            }
            aStats[at].AddModifier(value, modifierType, modName, repetitionBehavior);
        }
        
        public void RemoveArtefactModifier(EAAt at, EModifierType modifierType, string modName)
        {
            if (aStats.ContainsKey(at))
            {
                aStats[at].RemoveModifier(modifierType, modName);
            }
        }
        
        public bool AddArtefactModGroup(EAAt at)
        {
            if (aStats.ContainsKey(at))
            {
                return false;
            }
            else
            {
                aStats.Add(at, new BindableStat());
                return true;
            }
        }

        public void AddWeaponModifier(EWAt at, float value, EModifierType modifierType, string modName = "inventory",
            RepetitionBehavior repetitionBehavior = RepetitionBehavior.Return)
        {
            if (!wStats.ContainsKey(at))
            {
                AddWeaponModGroup(at);
            }
            wStats[at].AddModifier(value, modifierType, modName, repetitionBehavior);
        }
        
        public void RemoveWeaponModifier(EWAt at, EModifierType modifierType, string modName)
        {
            if (wStats.ContainsKey(at))
            {
                wStats[at].RemoveModifier(modifierType, modName);
            }
        }

        public bool AddWeaponModGroup(EWAt at)
        {
            if (wStats.ContainsKey(at))
            {
                return false;
            }
            else
            {
                wStats.Add(at, new BindableStat());
                return true;
            }
        }

        private void OnDisable()
        {
            foreach (var stat in aStats)
            {
                stat.Value.Reset();
            }
            aStats.Clear();
            foreach (var modGroup in wStats)
            {
                modGroup.Value.Reset();
            }
            wStats.Clear();
        }

        public BindableStat GetAAt(EAAt at)
        {
            if (!aStats.ContainsKey(at))
            {
                AddArtefactModGroup(at);
            }
            return aStats[at];
        }

        public BindableStat GetWAt(EWAt at)
        {
            if (!wStats.ContainsKey(at))
            {
                AddWeaponModGroup(at);
            }
            return wStats[at];
        }
        
        [Button("Init dicts")]
        public void InitDicts()
        {
            foreach (EAAt at in Enum.GetValues(typeof(EAAt)))
            {
                if (!aStats.ContainsKey(at))
                {
                    aStats.Add(at, new BindableStat());
                }
            }
        }
    }
}