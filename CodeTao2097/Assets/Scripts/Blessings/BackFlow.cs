using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 回流（水） 令修为在吸取时对接触的敌人造成回流伤害（受玩家移动速度影响）。
    /// BackFlow (Water) When absorbing the cultivation, it causes backflow damage to the enemy in contact (affected by the player's movement speed).
    /// </summary>
    public class BackFlow : Blessing
    {
        public Damager damager;
        
        public override void OnAdd()
        {
            base.OnAdd();
            if (!damager)
            {
                damager = this.GetComponentInDescendants<Damager>();
                MoveController moveController = Player.Instance.Link.GetComp<MoveController>();
                if (moveController)
                {
                    moveController.SPD.RegisterWithInitValue(value =>
                    {
                        damager.DMG.AddModifier(value - 1, EModifierType.Multiplicative, "SPD", RepetitionBehavior.Overwrite);
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