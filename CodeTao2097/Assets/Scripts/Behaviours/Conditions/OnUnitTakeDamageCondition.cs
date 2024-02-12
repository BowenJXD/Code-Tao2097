using System.Collections.Generic;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    ///  单位受到伤害条件，当指定的单位类型受到指定的伤害时，执行下一个行为。
    /// </summary>
    public class OnUnitTakeDamageCondition : SelectUnitBehaviour
    {
        public float damageAmount;
        /// <summary>
        /// 0 - 100
        /// </summary>
        public float chance = 100;
        public List<ElementType> damageTypes;
        public List<DamageTag> damageTags;
        public List<EntityTag> damagerTags;
        public List<EntityTag> attackerTags;
        public float knockBack;
        
        List<UnitController> units;

        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            units = sequence.Get<List<UnitController>>(BBKey.TARGETS);
            foreach (var unit in units)
            {
                if (unit.TryGetComp(out Defencer defencer))
                {
                    defencer.takeDamageAfter += damage => OnTakeDamage(unit, damage);
                }
            }
        }
        
        void OnTakeDamage(UnitController unit, Damage damage)
        {
            if (CheckCondition(damage))
            {
                Next();
            }
        }

        bool CheckCondition(Damage damage)
        {
            if (damage.Final < damageAmount) return false;
            if (!RandomUtil.Rand100(chance)) return false;
            if (damageTypes.Count > 0 && !damageTypes.Contains(damage.DamageElement)) return false;
            foreach (var damageTag in damageTags)
            {
                if (!damage.damageTags.Contains(damageTag)) return false;
            }
            if (damagerTags.Count > 0 && !Util.IsTagIncluded(damage.Median.tag, damagerTags)) return false;
            if (attackerTags.Count > 0 && !Util.IsTagIncluded(damage.Source.tag, attackerTags)) return false;
            if (damage.Knockback < knockBack) return false;
            return true;
        }
    }
}