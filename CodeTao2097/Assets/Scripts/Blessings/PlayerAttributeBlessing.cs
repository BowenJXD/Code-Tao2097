using System;
using System.Collections.Generic;

namespace CodeTao
{
    /// <summary>
    /// 影响玩家属性的祝福
    /// </summary>
    public class PlayerAttributeBlessing : Blessing
    {
        public List<AttributeMod> attributeMods = new List<AttributeMod>();
        
        protected AttributeController attributeController;

        public override void OnAdd()
        {
            base.OnAdd();
            if (!attributeController) attributeController = Container.GetComp<AttributeController>();
            foreach (var attributeMod in attributeMods)
            {
                if (attributeMod.aat != EAAt.Null)
                {
                    attributeController.AddArtefactModifier(attributeMod.aat, attributeMod.value, attributeMod.modifierType, name);
                }
                if (attributeMod.wat != EWAt.Null)
                {
                    attributeController.AddWeaponModifier(attributeMod.wat, attributeMod.value, attributeMod.modifierType, name);
                }
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            foreach (var attributeMod in attributeMods)
            {
                if (attributeMod.aat != EAAt.Null)
                {
                    attributeController.RemoveArtefactModifier(attributeMod.aat, attributeMod.modifierType, name);
                }
                if (attributeMod.wat != EWAt.Null)
                {
                    attributeController.RemoveWeaponModifier(attributeMod.wat, attributeMod.modifierType, name);
                }
            }
        }
    }
}