namespace CodeTao
{
    /// <summary>
    /// 战斗单位，包括敌人和玩家。会接受属性修改。
    /// </summary>
    public class CombatUnit : UnitController
    {
        public virtual BindableStat GetAAtMod(EAAt at)
        {
            return null;
        }

        public virtual void AddAAtMod(EAAt at, ModifierGroup modGroup)
        {
            BindableStat stat = GetAAtMod(at);
            if (stat != null)
            {
                stat.AddModifierGroup(modGroup);
            }
        }
    }
}