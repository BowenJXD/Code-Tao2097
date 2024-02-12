namespace CodeTao
{
    public class BBWAtModBehaviour : BehaviourNode
    {
        public string key;
        public EWAt at;
        public float modValueMultiplier;
        public EModifierType modType;
        public RepetitionBehavior repetitionBehavior = RepetitionBehavior.Overwrite;
        public IWAtSource source;

        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            source ??= GetComponentInParent<IWAtSource>();
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            float value = modValueMultiplier;
            if (sequence.TryGet(key, out float bbValue))
            {
                value *= bbValue;
            }
            source.GetWAt(at).AddModifier(value, modType, name, repetitionBehavior);
        }
    }
}