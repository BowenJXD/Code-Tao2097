﻿using System.Collections.Generic;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeTao
{
    public class Artefact : Item
    {
        [TabGroup("Content")]
        public List<ArtefactUpgradeMod> upgradeMods = new List<ArtefactUpgradeMod>();
        
        [HideInInspector] public AttributeController attributeController;

        public override void OnAdd()
        {
            attributeController = ComponentUtil.GetComponentFromUnit<AttributeController>(Container);
            base.OnAdd();
        }

        public override void Upgrade(int lvlIncrement = 1)
        {
            base.Upgrade(lvlIncrement);
            int newLevel = LVL.Value;

            foreach (var mod in upgradeMods)
            {
                if (mod.CheckCondition(newLevel))
                {
                    attributeController?.AddArtefactModifier(mod.attribute, mod.value, mod.modType, $"{GetType().Name} Level{newLevel}");
                    if (mod.exclusive) break;
                }
            }
        }
        
        public override string GetUpgradeDescription()
        {
            List<string> result = new List<string>();
            int newLevel = LVL.Value + 1;
            
            foreach (var mod in upgradeMods)
            {
                if (mod.CheckCondition(newLevel))
                {
                    result.Add(mod.GetDescription());
                    if (mod.exclusive) break;
                }
            }
            
            return base.GetUpgradeDescription() + result.StringJoin("\n");
        }
    }
}