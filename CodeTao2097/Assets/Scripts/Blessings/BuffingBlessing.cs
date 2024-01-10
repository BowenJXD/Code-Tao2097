using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// BuffingBlessing is a special blessing that can be used to buff the target if the specific type of element damage is dealt.
    /// </summary>
    public class BuffingBlessing : Blessing
    {
        [HideInInspector] public Attacker attacker;
        
        [HideInInspector] public Buff buffToApply;

        private ContentPool<Buff> _buffPool;

        public override void OnAdd()
        {
            base.OnAdd();
            attacker = Container.GetComp<Attacker>();
            attacker.DealDamageAfter += TryApplyBuff;
            buffToApply = this.GetComponentInDescendants<Buff>();
            _buffPool = new ContentPool<Buff>(buffToApply);
        }

        public void TryApplyBuff(Damage damage)
        {
            if (damage.Target.IsDead) return;
            BuffOwner target = damage.Target.GetComp<BuffOwner>();
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
            return relatedElements.Contains(damage.DamageElement);
        }
    }
}