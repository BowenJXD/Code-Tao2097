using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 石化buff：重复施加时无法刷新持续时间。附属者无法移动和攻击。
    /// </summary>
    public class PetrifiedBuff : Buff
    {
        private MoveController moveController;
        private Damager damager;
        private SpriteRenderer sp;
        private Color petrifiedColor = new Color(165, 42, 42, 1);
        private Color originalColor;
        
        public override void Init()
        {
            base.Init();
            
            moveController = Container.GetComp<MoveController>();
            damager = Container.GetComp<Damager>();
            sp = Container.GetComponentFromUnit<SpriteRenderer>();

            moveController.SPD.AddModifier(0, EModifierType.Multiplicative, name, RepetitionBehavior.Overwrite);
            damager.IsInCD = true;
            if (sp)
            {
                // TODO: change outline color
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            moveController.SPD.RemoveModifier(EModifierType.Multiplicative, name);
            damager.IsInCD = false;
            if (sp)
            {
                // TODO: change outline color
            }
        }
    }
}