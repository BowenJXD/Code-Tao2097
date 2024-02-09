using QFramework;
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
        Rigidbody2D rb;
        RigidbodyConstraints2D oldConstraints;
        private Color oldColor;
        private float oldWidth;
        private bool oldEnabled;
        private SpriteRenderer sp;
        Outliner outliner;
        
        public override void Init()
        {
            base.Init();
            
            moveController = Container.GetComp<MoveController>();
            damager = Container.GetComp<Damager>();
            sp = Container.GetComponentFromUnit<SpriteRenderer>();
            if (sp) outliner = sp.GetOrAddComponent<Outliner>();

            moveController.SPD.AddModifier(0, EModifierType.Multiplicative, name, RepetitionBehavior.Overwrite, true);
            damager.IsInCD = true;
            Rigidbody2D rb = Container.GetComponentFromUnit<Rigidbody2D>();
            if (rb)
            {
                oldConstraints = rb.constraints;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            oldColor = outliner.OutlineColor;
            oldWidth = outliner.OutlineWidth;
            oldEnabled = outliner.enabled;
            
            outliner.OutlineColor = elementType.GetColor();
            outliner.OutlineWidth = 0.1f;
            outliner.enabled = true;
        }

        public override void OnRemove()
        {
            base.OnRemove();
            moveController.SPD.RemoveModifier(EModifierType.Multiplicative, name);
            damager.IsInCD = false;
            if (rb)
            {
                rb.constraints = oldConstraints;
            }

            outliner.OutlineColor = oldColor;
            outliner.OutlineWidth = oldWidth;
            outliner.enabled = oldEnabled;
        }
    }
}