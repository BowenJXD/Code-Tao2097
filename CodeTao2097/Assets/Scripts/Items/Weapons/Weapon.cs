using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    [Serializable]
    public class UpgradeMod
    {
        /// <summary>
        /// Minimum level (inclusive) to unlock this modifier, 0 for default, negative to be overriden.
        /// </summary>
        public int minLevel = 0;
        /// <summary>
        /// Maximum level (inclusive) to unlock this modifier, 0 for default, negative to be overriden.
        /// </summary>
        public int maxLevel = 0;
        public EWAt attribute;
        public EModifierType modType;
        public float value;
        
        public bool CheckCondition(int level, bool triggered = false)
        {
            bool result = false;
            maxLevel = Mathf.Abs(maxLevel) < Mathf.Abs(minLevel) ? minLevel : maxLevel;
            if (triggered)
            {
                if (minLevel > 0 && maxLevel > 0)
                {
                    if (level >= Mathf.Abs(minLevel) && level <= Mathf.Abs(maxLevel))
                    {
                        result = true;
                    }
                }
            }
            else
            {
                if (level >= Mathf.Abs(minLevel) && level <= Mathf.Abs(maxLevel))
                {
                    result = true;
                }

                if (minLevel == 0 && maxLevel == 0)
                {
                    result = true;
                }
            }

            return result;
        }
        
        public static Comparison<UpgradeMod> Comparison()
        {
            return (a, b) =>
            {
                int absMinLevelComparison = Mathf.Abs(b.minLevel).CompareTo(Mathf.Abs(a.minLevel));

                if (absMinLevelComparison == 0)
                {
                    // If absolute minLevels are equal, positive comes before negative
                    return b.minLevel.CompareTo(a.minLevel);
                }
                else
                {
                    return absMinLevelComparison;
                }
            };
        }

        public BindableStat AddModifier(BindableStat stat, int newLevel)
        {
            stat.AddModifier($"Level{newLevel}", value, modType);

            return stat;
        }

        public string GetDescription()
        {
            string result = "";
            char sign = value > 0 ? '+' : '-';
            switch (modType)
            {
                case EModifierType.Basic:
                    result += $" base {attribute} {sign} {value}.";
                    break; 
                case EModifierType.Additive:
                    result += $" {attribute} {sign} {value}.";
                    break;
                case EModifierType.MultiAdd:
                    result += $" {attribute} {sign} {value * 100} %.";
                    break;
                case EModifierType.Multiplicative:
                    result += $" {attribute} * {(1 + value) * 100} %.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
    }
    
    public class Weapon : Item
    {
        [SerializeField]
        public SerializableDictionary<EWAt, BindableStat> ats = new SerializableDictionary<EWAt, BindableStat>()
        {
            {EWAt.Damage, new BindableStat(10)}, 
            {EWAt.Amount, new BindableStat(1)},
            {EWAt.Duration, new BindableStat(5).SetMinValue(0.1f)},
            {EWAt.Speed, new BindableStat(4)},
            {EWAt.Cooldown, new BindableStat(2).SetMinValue(0.1f)},
            {EWAt.Area, new BindableStat(0)}
        };
        
        public BindableProperty<int> shotsToReload = new BindableProperty<int>(0);
        public BindableStat reloadTime = new BindableStat(0);
        
        public BindableProperty<float> attackRange = new BindableProperty<float>(10);
        
        [HideInInspector] public Attacker attacker;
        
        [HideInInspector] public Damager damager;

        protected LoopTask fireLoop;
        
        public List<UpgradeMod> upgradeMods = new List<UpgradeMod>();

        protected override void Start()
        {
            base.Start();
            
            damager = ComponentUtil.GetComponentInDescendants<Damager>(this);
            ats[EWAt.Damage].RegisterWithInitValue(dmg =>
            {
                damager.DMG.AddModifier("weapon", dmg, EModifierType.Additive, ERepetitionBehavior.Overwrite);
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            // setup fire loop
            fireLoop = new LoopTask(this, ats[EWAt.Cooldown], Fire, StartReload);
            if (shotsToReload.Value > 0)
            {
                shotsToReload.RegisterWithInitValue(count =>
                {
                    fireLoop.SetCountCondition(count);
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
            fireLoop.Start();
            ats[EWAt.Cooldown].RegisterWithInitValue(interval =>
            {
                fireLoop.LoopInterval = ats[EWAt.Cooldown];
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            // Add to inventory
            UnitController unitController = ComponentUtil.GetComponentInAncestors<UnitController>(this);
            if (unitController)
            {
                Container<Item> itemContainer = ComponentUtil.GetComponentInDescendants<Inventory>(unitController);
                if (itemContainer != null)
                {
                    ActionKit.DelayFrame(1, () =>
                    {
                        if (!this) return;
                        itemContainer.AddContent(this);
                    }).Start(this);
                }
            }
        }

        public virtual void Fire()
        {
            
        }

        public virtual void StartReload()
        {
            ActionKit.Delay(reloadTime.Value, () =>
            {
                Reload();
            }).Start(this);
        }
        
        public virtual void Reload()
        {
            fireLoop.Start();
        }

        public override void Upgrade(int lvlIncrement = 1)
        {
            base.Upgrade(lvlIncrement);
            int newLevel = LVL.Value;
            
            // sort upgradeMods by minLevel in descending order
            upgradeMods.Sort(UpgradeMod.Comparison());
            
            bool triggered = false;
            foreach (var mod in upgradeMods)
            {
                if (mod.CheckCondition(newLevel, triggered))
                {
                    ats[mod.attribute].AddModifier($"Level{newLevel}", mod.value, mod.modType);
                    triggered = true;
                }
            }
        }
        
        public override string GetDescription()
        {
            List<string> result = new List<string>();
            int newLevel = LVL.Value + 1;
            
            // sort upgradeMods by minLevel in descending order
            upgradeMods.Sort(UpgradeMod.Comparison());
            
            bool triggered = false;
            foreach (var mod in upgradeMods)
            {
                if (mod.CheckCondition(newLevel, triggered))
                {
                    result.Add($"{GetType().Name} ({newLevel})" + mod.GetDescription());
                    triggered = true;
                }
            }
            
            return result.StringJoin("\n");
        }

        public virtual void Attack(Defencer defencer)
        {
            Damage dmg = DamageManager.Instance.ExecuteDamage(damager, defencer, attacker);
        }
        
        
        /// <summary>
        /// Get available targets in attackRange in ascending order of distance
        /// </summary>
        /// <returns></returns>
        public List<Defencer> GetTargets(Collider2D colUsed = null)
        {
            List<Collider2D> colliders = new List<Collider2D>();
            if (colUsed)
            {
                Physics2D.OverlapCollider(colUsed, new ContactFilter2D().NoFilter(), colliders);
            }
            else
            {
                colliders = Physics2D.OverlapCircleAll(transform.position, attackRange.Value).ToList();
            }
            
            SortedList<float, Defencer> targetDistances = new SortedList<float, Defencer>();
            foreach (var col in colliders)
            {
                Defencer target = DamageManager.Instance.ColToDef(damager, col);
                if (target)
                {
                    float targetDistance = Vector2.Distance(transform.position, col.transform.position);
                    targetDistances.Add(targetDistance, target);
                }
            }

            return targetDistances.Values.ToList();
        }
    }
}