using CodeTao;
using QFramework;

namespace CodeTao
{
    /// <summary>
    /// Modifies an attribute of the unit temporarily.
    /// </summary>
    public class AttributeBuff : Buff
    {
        public BindableProperty<float> modValue;
        public EAAt attribute;
        public EModifierType modifierType;

        public override void Init()
        {
            base.Init();
            LVL.RegisterWithInitValue(value =>
            {
                buffOwner?.attributeController?.AddArtefactModifier(attribute, modValue * value, modifierType, name,
                    RepetitionBehavior.Overwrite);
            }).UnRegisterWhenGameObjectDestroyed(this);
        }
        
        public override void OnRemove()
        {
            base.OnRemove();
            buffOwner?.attributeController?.RemoveArtefactModifier(attribute, modifierType, name);
        }
    }
}