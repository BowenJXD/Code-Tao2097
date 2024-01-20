using System.Collections.Generic;

namespace CodeTao
{
    public class EnemyAttributeBlessing : Blessing
    {
        public List<AttributeMod> attributeMods = new List<AttributeMod>();
        List<Enemy> enemies = new List<Enemy>();

        public override void OnAdd()
        {
            base.OnAdd();
            EnemyManager.Instance.AddOnSpawnAction(OnSpawn);
        }

        void OnSpawn(Enemy enemy)
        {
            AttributeController attributeController = enemy.GetComp<AttributeController>();
            if (!attributeController) return;
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
            foreach (var enemy in enemies)
            {
                AttributeController attributeController = enemy.GetComp<AttributeController>();
                if (!attributeController) continue;
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
}