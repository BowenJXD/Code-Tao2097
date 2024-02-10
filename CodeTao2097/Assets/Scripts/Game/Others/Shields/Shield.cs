using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace CodeTao
{
    /// <summary>
    /// 阻挡伤害的护盾，能在触碰时造成伤害。
    /// </summary>
    public class Shield : UnitController, IWeaponDerivative
    {
        private SpriteRenderer shieldSp;
        private Attacker attacker;
        private Defencer protectingDefencer;
        private Damager damager;
        private Collider2D shieldCol;
        private LoopTaskController taskController;
        
        public Weapon weapon { get; set; }
        public void SetWeapon(Weapon newWeapon, Damager newDamager)
        {
            weapon = newWeapon;
            damager = newDamager;
            attacker = weapon.Container.GetComp<Attacker>();
            protectingDefencer = weapon.Container.GetComp<Defencer>();
        }

        public override void SetUp()
        {
            base.SetUp();
            shieldSp = this.GetComponentInDescendants<SpriteRenderer>(true);
            shieldCol = this.GetComponentInDescendants<Collider2D>(true);
            damager = this.GetComponentInDescendants<Damager>(true);
            taskController = this.GetComp<LoopTaskController>();
        }

        public override void Init()
        {
            base.Init();
            if (protectingDefencer)
            {
                protectingDefencer.IsInCD = true;
            }
            if (damager)
            {
                shieldCol.OnTriggerEnter2DEvent(col =>
                {
                    Defencer def = DamageManager.Instance.ColToDef(damager, col);
                    if (def)
                    {
                        DamageManager.Instance.ExecuteDamage(damager, def, attacker);
                    }
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
            taskController.AddFinish(Deinit);
            float duration = taskController.duration;
            if (duration > 2){
                ActionKit.Delay(duration - 2,
                    () => FlashSprite(shieldSp, Color.clear, 2)).Start(this);
            }
        }

        public override void Deinit()
        {
            base.Deinit();
            if (protectingDefencer)
            {
                protectingDefencer.IsInCD = false;
            }
        }

        void FlashSprite(SpriteRenderer spriteRenderer, Color flashColor, float flashDuration)
        {
            // Create a sequence of tweens to flash the sprite
            Sequence flashSequence = DOTween.Sequence();
            const float flashInterval = 0.5f;
            
            int loopCount = Mathf.CeilToInt(flashDuration / flashInterval * 2);
            int intervalCount = 2 * loopCount + 1;
            float remainder = flashDuration % flashInterval;
            float divisor = flashDuration - remainder;
            
            // wait for a short time before starting the flash
            flashSequence.AppendInterval(remainder);
            
            for (int i = 0; i < loopCount; i++)
            {
                flashSequence.Append(spriteRenderer.DOColor(flashColor, divisor / intervalCount));
                // exclude the last loop
                if (i < loopCount - 1)
                {
                    flashSequence.Append(spriteRenderer.DOColor(Color.white, divisor /intervalCount));
                }
                else
                {
                    flashSequence.Append(spriteRenderer.DOColor(Color.white, 0));
                }
            }
            
            flashSequence.Play();
        }

    }
}