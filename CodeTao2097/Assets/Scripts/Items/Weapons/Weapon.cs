using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 武器的基类，包含武器的基本属性，以及武器的基本功能
    /// </summary>
    public class Weapon : Item
    {
        public ElementType elementType
        {
            get => relatedElements.FirstOrDefault();
            set => relatedElements = new List<ElementType>() {value};
        }

        public BindableStat damage = new BindableStat(10);
        public BindableStat amount = new BindableStat(1);
        public BindableStat duration = new BindableStat(5).SetMinValue(0.1f);
        public BindableStat speed = new BindableStat(4);
        public BindableStat cooldown = new BindableStat(2).SetMinValue(0.1f);
        public BindableStat area = new BindableStat(1).SetMinValue(0.1f);
        public BindableStat knockBack = new BindableStat(0);
        public BindableStat effectHitRate = new BindableStat(50).SetMaxValue(100f);

        public BindableProperty<int> shotsToReload = new BindableProperty<int>(0);
        public BindableStat reloadTime = new BindableStat(0);
        public BindableStat attackRange = new BindableStat(10);
        
        [HideInInspector] public Attacker attacker;
        [HideInInspector] public List<Damager> damagers;

        protected LoopTask fireLoop;
        
        protected WeaponSelector weaponSelector;
        protected List<WeaponExecutor> weaponExecuters = new List<WeaponExecutor>();
        protected List<EntityType> attackingTypes = new List<EntityType>();
        
        [TabGroup("Content")]
        public List<WeaponUpgradeMod> upgradeMods = new List<WeaponUpgradeMod>();

        public override void Init()
        {
            base.Init();

            if (!weaponSelector) weaponSelector = GetComponent<WeaponSelector>();
            if (!weaponSelector) weaponSelector = this.GetComponentInDescendants<WeaponSelector>();
            weaponSelector.Init(this);
            
            if (weaponExecuters.Count <= 0) weaponExecuters = this.GetComponentsInDescendants<WeaponExecutor>(true).ToList();
            weaponExecuters.ForEach(we => we.Init(this));
            
            if (!attacker) attacker = Container.GetComp<Attacker>();
            if (damagers.Count <= 0) damagers = this.GetComponentsInDescendants<Damager>(true).ToList();
            damagers.ForEach(damager => attackingTypes.AddRange(damager.damagingTags));
            
            reloadTime.AddModifierGroups(cooldown.ModGroups);
            
            // setup fire loop
            fireLoop = new LoopTask(this, cooldown, Fire, StartReload);
            if (shotsToReload.Value > 0)
            {
                shotsToReload.RegisterWithInitValue(count =>
                {
                    fireLoop.SetCountCondition(count);
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
            fireLoop.Start();
            cooldown.RegisterWithInitValue(interval =>
            {
                fireLoop.LoopInterval = interval;
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public virtual void Fire()
        {
            List<Vector3> globalPositions = weaponSelector.GetGlobalPositions();
            ISequence actionSequence = ActionKit.Sequence();
            for (int i = 0; i < weaponExecuters.Count; i++)
            {
                WeaponExecutor weaponExecutor = weaponExecuters[i];
                actionSequence.Callback(() => weaponExecutor.Execute(globalPositions));
                if (i < weaponExecuters.Count - 1)
                {
                    actionSequence.Condition(() => weaponExecutor.next);
                }
            }
            actionSequence.Start(this);
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

            foreach (var mod in upgradeMods)
            {
                if (mod.CheckCondition(newLevel))
                {
                    GetWAtStat(mod.attribute).AddModifier(mod.value, mod.modType, $"Level{LVL + 1}");
                    if (mod.exclusive) break;
                }
            }
        }
        
        public virtual BindableStat GetWAtStat(EWAt at)
        {
            BindableStat statToMod = null;
            switch (at)
            {
                case EWAt.Damage:
                    statToMod = damage;
                    break;
                case EWAt.Cooldown:
                    statToMod = cooldown;
                    break;
                case EWAt.Area:
                    statToMod = area;
                    break;
                case EWAt.Amount:
                    statToMod = amount;
                    break;
                case EWAt.Duration:
                    statToMod = duration;
                    break;
                case EWAt.Speed:
                    statToMod = speed;
                    break;
                case EWAt.Range:
                    statToMod = attackRange;
                    break;
                case EWAt.Knockback:
                    statToMod = knockBack;
                    break;
                case EWAt.EffectHitRate:
                    statToMod = effectHitRate;
                    break;
            }

            return statToMod;
        }
        
        public override string GetUpgradeDescription()
        {
            List<string> result = new List<string>();
            int newLevel = LVL.Value + 1;
            
            foreach (var mod in upgradeMods)
            {
                if (mod.CheckCondition(newLevel))
                {
                    result.Add(mod.GetDescription());
                    if (mod.exclusive) break;
                }
            }
            
            return base.GetUpgradeDescription() + result.StringJoin("\n");
        }

        /// <summary>
        /// Get available targets in attackRange in ascending order of distance
        /// </summary>
        /// /// <param name="range">default to be attackRange</param>
        /// <param name="colUsed"></param>
        /// <returns>Sorted in ascending order of distance to the ordering position.</returns>
        public List<Defencer> GetTargets(float range = 0, Collider2D colUsed = null)
        {
            List<Collider2D> colliders = new List<Collider2D>();
            if (range == 0)
            {
                range = attackRange;
            }
            if (colUsed)
            {
                Physics2D.OverlapCollider(colUsed, new ContactFilter2D().NoFilter(), colliders);
            }
            else
            {
                colliders = Physics2D.OverlapCircleAll(transform.position, range).ToList();
            }

            SortedList<float, Defencer> targetDistances = new SortedList<float, Defencer>();
            foreach (var col in colliders)
            {
                Defencer target = null;
                UnitController unitController = col.GetUnit();
                if (unitController)
                {
                    target = unitController.GetComp<Defencer>();
                    if (Util.IsTagIncluded(unitController.tag, attackingTypes) && target)
                    {
                        float targetDistance = Vector2.Distance(transform.position, col.transform.position);
                        targetDistances.Add(targetDistance, target);
                    }
                }
            }

            return targetDistances.Values.ToList();
        }
    }
}