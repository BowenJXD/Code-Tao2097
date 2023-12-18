namespace CodeTao
{
    public class CombatUnit : UnitController
    {
        public virtual BindableStat GetAAtMod(EAAt at)
        {
            return null;
        }

        public virtual void AddAAtMod(EAAt at, ModifierGroup modGroup)
        {
            GetAAtMod(at).AddModifierGroup(modGroup);
        }
    }
}