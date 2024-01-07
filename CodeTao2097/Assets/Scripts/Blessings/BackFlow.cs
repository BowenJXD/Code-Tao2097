using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class BackFlow : Blessing
    {
        public Damager damager;
        
        public override void OnAdd()
        {
            base.OnAdd();
            if (!damager)
            {
                damager = this.GetComponentInDescendants<Damager>();
                MoveController moveController = Player.Instance.MoveController;
                if (moveController)
                {
                    moveController.SPD.RegisterWithInitValue(value =>
                    {
                        damager.DMG.AddModifier(value - 1, EModifierType.Multiplicative, "SPD", ERepetitionBehavior.Overwrite);
                    }).UnRegisterWhenGameObjectDestroyed(this);
                }
            }

            ExpGenerator.Instance.onUnitGet += unit =>
            {
                unit.interactableCol.IncludeLayer(ELayer.Enemy);
                unit.rb2D.IncludeLayer(ELayer.Enemy);
                unit.interactableCol.OnTriggerEnter2DEvent(col =>
                {
                    Attack(col);
                }).UnRegisterWhenGameObjectDestroyed(unit.gameObject);
            };
        }

        void Attack(Collider2D col)
        {
            Defencer defencer = DamageManager.Instance.ColToDef(damager, col);
            if (defencer)
            {
                DamageManager.Instance.ExecuteDamage(damager, defencer);
            }
        }
    }
}