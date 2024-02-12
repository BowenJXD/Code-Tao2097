using QFramework;
using Schema.Internal.Types;

namespace CodeTao
{
    public class BBAAtModBehaviour : BehaviourNode
    {
        public string key;
        public EAAt at;
        public float modValueMultiplier;
        public EModifierType modType;
        public RepetitionBehavior repetitionBehavior = RepetitionBehavior.Overwrite;
        public IAAtSource source;

        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            source = this.GetComponentFromUnit<AttributeController>();
            if (source == null)
            {
                source = sequence.Get<UnitController>(BBKey.OWNER)?.GetComp<AttributeController>();
            }
            if (source == null)
            {
                if (sequence.TryGet(BBKey.ITEM, out Item item))
                {
                    var container = item.Container;
                    if (container)
                    {
                        source = container.GetComp<AttributeController>();
                    }
                }
            }
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            float value = modValueMultiplier;
            if (sequence.TryGet(key, out float bbValue))
            {
                value *= bbValue;
            }
            source.GetAAt(at).AddModifier(value, modType, name, repetitionBehavior);
            // LogKit.I("Add Modifier: " + name + " " + value);
        }
    }
}