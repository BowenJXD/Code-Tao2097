using System;
using QFramework;

namespace CodeTao
{
    [Obsolete]
    public class DotBuffingBlessing : BuffingBlessing
    {
        protected Damager damager;
        
        public EAAt baseAttribute;
        public EModifierType modType;

        private void Awake()
        {
            damager = ComponentUtil.GetComponentInDescendants<Damager>(this);
        }

        public override void OnAdd()
        {
            base.OnAdd();
            
            if (baseAttribute != EAAt.NULL)
            {
                CombatUnit combatUnit = ComponentUtil.GetComponentInAncestors<CombatUnit>(Container);
                combatUnit.GetAAtMod(baseAttribute).RegisterWithInitValue(value =>
                {
                    damager.DMG.AddModifier(value, modType, name, ERepetitionBehavior.Overwrite);
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
        }

        public override Buff ApplyBuff(BuffOwner target)
        {
            Buff buff = base.ApplyBuff(target);
            DotBuff dotBuff = (DotBuff)buff;
            if (dotBuff) dotBuff.damager = damager;
            return buff;
        }
    }
}