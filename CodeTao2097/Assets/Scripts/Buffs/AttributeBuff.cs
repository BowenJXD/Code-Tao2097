using CodeTao;
using QFramework;

namespace CodeTao
{
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
                    ERepetitionBehavior.Overwrite);
            }).UnRegisterWhenGameObjectDestroyed(this);

        }
        
        public override void Remove()
        {
            base.Remove();
            buffOwner?.attributeController?.RemoveArtefactModifier(attribute, modifierType, name);
        }
    }
}