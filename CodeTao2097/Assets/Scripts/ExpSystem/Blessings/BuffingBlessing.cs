using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class BuffingBlessing : Blessing
    {
        public ElementType buffingElement;
        
        [HideInInspector] public Attacker attacker;
        
        [HideInInspector] public Buff buffToApply;

        private ContentPool<Buff> _buffPool;

        public override void OnAdd()
        {
            base.OnAdd();
            attacker = ComponentUtil.GetComponentFromUnit<Attacker>(Container);
            attacker.DealDamageAfter += TryApplyBuff;
            buffToApply = ComponentUtil.GetComponentInDescendants<Buff>(this);
            _buffPool = new ContentPool<Buff>(buffToApply);
        }

        public void TryApplyBuff(Damage damage)
        {
            if (damage.Target.IsDead) return;
            BuffOwner target = ComponentUtil.GetComponentFromUnit<BuffOwner>(damage.Target);
            if (target && CheckCondition(damage))
            {
                ApplyBuff(target);
            }
        }
        
        public virtual Buff ApplyBuff(BuffOwner target)
        {
            Buff buff = _buffPool.Get().Parent(this);
            if (!buff.AddToContainer(target))
            {
                _buffPool.Release(buff);
            }
            else
            {
                buff.RemoveAfter += buffRemoved =>
                {
                    _buffPool.Release(buffRemoved);
                };
            }
            return buff;
        }

        public bool CheckCondition(Damage damage)
        {
            return damage.DamageElement.Type == buffingElement;
        }
    }
}