using Buffs;
using Buffs.ElementBuffs;
using UnityEngine;

namespace CodeTao
{
    public class BuffingBlessing : Blessing
    {
        [HideInInspector] public Attacker attacker;
        
        [HideInInspector] public Content<Buff> buffToApply;

        private BuffPool<BurningBuff> _buffPool;

        public override void OnAdd()
        {
            base.OnAdd();
            attacker = ComponentUtil.GetComponentFromUnit<Attacker>(Container);
            attacker.DealDamageAfter += ApplyBuff;
            buffToApply = ComponentUtil.GetComponentInDescendants<Buff>(this);
            _buffPool = new BuffPool<BurningBuff>((BurningBuff) buffToApply);
        }

        public void ApplyBuff(Damage damage)
        {
            if (damage.Target.IsDead) return;
            BuffOwner target = ComponentUtil.GetComponentFromUnit<BuffOwner>(damage.Target);
            if (target && CheckCondition(damage))
            {
                BurningBuff buff = _buffPool.Get();
                buff.AddToContainer(target);
            }
        }

        public bool CheckCondition(Damage damage)
        {
            return damage.DamageElement.Type == ElementType.Fire;
        }
    }
}