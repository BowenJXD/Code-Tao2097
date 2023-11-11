using QFramework;

namespace QFramework
{
    public static class BuffEx
    {
        public static void ExcuteBuff(this IBuffOwner owner, BuffConfig config, int level)
        {
            float source = owner.GetSourceValue(config.Name);
            owner.CalcPassive(config, level, ref source);
            owner.SetValue(config.Name, source);
        }
        public static void ExcuteBuff(this IBuffOwner owner, BuffConfig config, Buff buff)
        {
            owner.ExcuteBuff(config, buff.Level);
        }
        public static void CalcPassive(this IBuffOwner owner, BuffConfig config, int level, ref float source)
        {
            float delta = config.Value * level;
            source = config.Operator switch
            {
                E_Operator.Add => source + delta,
                E_Operator.Sub => source - delta,
                E_Operator.Mul => source * (1 + delta),
                E_Operator.Div => source * (1 - delta),
                _ => source
            };
        }
    }
}