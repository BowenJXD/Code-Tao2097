using Buffs;

namespace CodeTao
{
    public class BuffingBlessing : Blessing
    {
        public Attacker attacker;
        
        public Content<Buff> buffToApply;

        public void Init()
        {
            attacker.DealDamageAfter += ApplyBuff;
        }

        public void ApplyBuff(Damage damage)
        {
            BuffOwner target = ComponentUtil.GetComponentFromUnit<BuffOwner>(damage.Target);
            if (target && CheckCondition(damage))
            {
                buffToApply.AddToContainer(target);
            }
        }

        public bool CheckCondition(Damage damage)
        {
            return damage.DamageElement.Type == ElementType.Fire;
        }
    }
}