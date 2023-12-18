using UnityEngine;

namespace CodeTao
{
    public class BuffingBlessing : Blessing
    {
        public ElementType buffingElement;
        
        [HideInInspector] public Attacker attacker;
        
        [HideInInspector] public Content<Buff> buffToApply;

        private BuffPool<Buff> _buffPool;

        public override void OnAdd()
        {
            base.OnAdd();
            attacker = ComponentUtil.GetComponentFromUnit<Attacker>(Container);
            attacker.DealDamageAfter += ApplyBuff;
            buffToApply = ComponentUtil.GetComponentInDescendants<Buff>(this);
            _buffPool = new BuffPool<Buff>((Buff) buffToApply);
        }

        public void ApplyBuff(Damage damage)
        {
            if (damage.Target.IsDead) return;
            BuffOwner target = ComponentUtil.GetComponentFromUnit<BuffOwner>(damage.Target);
            if (target && CheckCondition(damage))
            {
                Buff buff = _buffPool.Get();
                buff.AddToContainer(target);
            }
        }

        public bool CheckCondition(Damage damage)
        {
            return damage.DamageElement.Type == buffingElement;
        }
    }
}