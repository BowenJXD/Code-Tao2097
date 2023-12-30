using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class Artefact : Item
    {
        public List<ArtefactUpgradeMod> upgradeMods = new List<ArtefactUpgradeMod>();
        
        public AttributeController attributeController;

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
        
        public override string GetDescription()
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
            
            return base.GetDescription() + result.StringJoin("\n");
        }
    }
}