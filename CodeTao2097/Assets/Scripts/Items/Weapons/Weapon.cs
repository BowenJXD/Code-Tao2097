using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class Weapon : Item, IWAtSource, IWAtReceiver
    {
        public ElementType ElementType
        {
            get => relatedElements.FirstOrDefault();
            set => relatedElements = new List<ElementType>() {value};
        }

        public BindableStat damage = new BindableStat(10);
        public BindableStat amount = new BindableStat(1);
        public BindableStat duration = new BindableStat(5);
        public BindableStat speed = new BindableStat(4);
        public BindableStat cooldown = new BindableStat(2);
        public BindableStat area = new BindableStat(1);
        
        public BindableStat knockBack = new BindableStat(0);
        public BindableStat effectHitRate = new BindableStat(50).SetMaxValue(100f);
        public BindableStat attackRange = new BindableStat(10);
        
        [HideInInspector] public Attacker attacker;
        [HideInInspector] public List<Damager> damagers;

        protected WeaponSelector weaponSelector;
        protected List<WeaponExecutor> weaponExecuters = new List<WeaponExecutor>();
        protected List<EntityType> attackingTypes = new List<EntityType>();
        
        [BoxGroup("Content")]
        public List<WeaponUpgradeMod> upgradeMods = new List<WeaponUpgradeMod>();

        public bool individualFire = true;

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
            foreach (var damager in damagers)
            {
                attackingTypes.AddRange(damager.damagingTags);
                damager.damageElementType = ElementType;
            }
            
            Receive(Container.GetComp<AttributeController>().As<IWAtSource>());
            IWAtReceiver[] wAtReceivers = this.GetComponentsInChildren<IWAtReceiver>(true);
            this.As<IWAtSource>().Transmit(wAtReceivers);

            ActionKit.Delay(duration / 2f, Fire).Start(this);
        }

        public virtual void Fire()
        {
            List<Vector3> globalPositions = weaponSelector.GetGlobalPositions();
            ISequence fireSequence = ActionKit.Sequence();
            for (int i = 0; i < weaponExecuters.Count; i++)
            {
                WeaponExecutor weaponExecutor = weaponExecuters[i];
                fireSequence.Coroutine(() => weaponExecutor.ExecuteCoroutine(globalPositions));
            }

            if (individualFire)
            {
                ActionKit.Delay(cooldown, Fire).Start(this);
            }
            else
            {
                fireSequence.Delay(cooldown, Fire);
            }
            
            fireSequence.Start(this);
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
                        float targetDistance = Vector2.SqrMagnitude(col.transform.position - transform.position);
                        while (targetDistances.ContainsKey(targetDistance))
                        {
                            targetDistance += 0.0001f;
                        }

                        targetDistances.Add(targetDistance, target);
                    }
                }
            }

            return targetDistances.Values.ToList();
        }
        
        public override void AlterLVL(int lvlIncrement = 1)
        {
            base.AlterLVL(lvlIncrement);
            int newLevel = LVL.Value;

            foreach (var mod in upgradeMods)
            {
                if (mod.CheckCondition(newLevel))
                {
                    GetWAt(mod.attribute).AddModifier(mod.value, mod.modType, $"Level{LVL + 1}");
                    if (mod.exclusive) break;
                }
            }
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

        private void OnEnable()
        {
            foreach (EWAt at in Enum.GetValues(typeof(EWAt)))
            {
                if (at == EWAt.Null) continue;
                BindableStat stat = GetWAt(at);
                if (stat != null){
                    stat.Init();
                }
            }
        }

        private void OnDisable()
        {
            foreach (EWAt at in Enum.GetValues(typeof(EWAt)))
            {
                if (at == EWAt.Null) continue;
                BindableStat stat = GetWAt(at);
                if (stat != null){
                    stat.Reset();
                }
            }
        }

        public void LoadAttributeData(ConfigData data)
        {
            Dictionary<string, string> dataDict = data.GetDataById(name);
            if (dataDict == null)
            {
                Debug.LogError($"No data found for {name}");
                return;
            }
            
            foreach (EWAt at in Enum.GetValues(typeof(EWAt)))
            {
                if (!dataDict.TryGetValue(at.ToString(), out string value)) continue;
                
                try{
                    BindableStat stat = new BindableStat(float.Parse(value));
                    SetWAt(at, stat);
                } catch
                {
                    Debug.LogError($"Error parsing {value} to float for {name} {at}");
                }
            }
        }
        
        public void SaveAttributeData(ConfigData data)
        {
            Dictionary<string, string> dataDict = new Dictionary<string, string>();
            data.SetDataById(name, dataDict);
            
            dataDict["Id"] = name;
            foreach (EWAt at in Enum.GetValues(typeof(EWAt)))
            {
                if (at == EWAt.Null) continue;
                BindableStat stat = GetWAt(at);
                if (stat != null){
                    dataDict[at.ToString()] = stat.Value.ToString(CultureInfo.CurrentCulture);
                }
            }
        }

        public void LoadUpgradeData(ConfigData data)
        {
            List<Dictionary<string, string>> dataDicts = data.GetDatasById(name);
            if (dataDicts == null)
            {
                Debug.LogError($"No data found for {name}");
                return;
            }
            
            upgradeMods.Clear();
            foreach (var dataDict in dataDicts)
            {
                WeaponUpgradeMod mod = new WeaponUpgradeMod();
                try
                {
                    mod.levels = dataDict["Levels"];
                    string at = dataDict["Attribute"];
                    EWAt attribute = (EWAt)Enum.Parse(typeof(EWAt), at);
                    mod.attribute = attribute;
                    mod.value = float.Parse(dataDict["Value"]);
                    mod.modType = (EModifierType)Enum.Parse(typeof(EModifierType), dataDict["ModType"]);
                    mod.exclusive = bool.Parse(dataDict["Exclusive"]);
                    upgradeMods.Add(mod);
                } catch
                {
                    Debug.LogError($"Error parsing {dataDict["Attribute"]} to EWAt for {name}");
                    continue;
                }
            }
        }
        
        public void SaveUpgradeData(ConfigData data)
        {
            List<Dictionary<string, string>> dataDicts = new List<Dictionary<string, string>>();
            data.SetDatasById(name, dataDicts);
            
            foreach (var upgradeMod in upgradeMods)
            {
                Dictionary<string, string> dataDict = new Dictionary<string, string>();
                dataDict["Id"] = name;
                dataDict["Levels"] = upgradeMod.levels;
                dataDict["Attribute"] = upgradeMod.attribute.ToString();
                dataDict["Value"] = upgradeMod.value.ToString(CultureInfo.CurrentCulture);
                dataDict["ModType"] = upgradeMod.modType.ToString();
                dataDict["Exclusive"] = upgradeMod.exclusive.ToString();
                dataDicts.Add(dataDict);
            }
        }

        public BindableStat GetWAt(EWAt at)
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
                case EWAt.KnockBack:
                    statToMod = knockBack;
                    break;
                case EWAt.EffectHitRate:
                    statToMod = effectHitRate;
                    break;
            }

            return statToMod;
        }
        
        public void SetWAt(EWAt at, BindableStat stat)
        {
            switch (at)
            {
                case EWAt.Damage:
                    damage = stat;
                    break;
                case EWAt.Cooldown:
                    cooldown = stat;
                    break;
                case EWAt.Area:
                    area = stat;
                    break;
                case EWAt.Amount:
                    amount = stat;
                    break;
                case EWAt.Duration:
                    duration = stat;
                    break;
                case EWAt.Speed:
                    speed = stat;
                    break;
                case EWAt.Range:
                    attackRange = stat;
                    break;
                case EWAt.KnockBack:
                    knockBack = stat;
                    break;
                case EWAt.EffectHitRate:
                    effectHitRate = stat;
                    break;
            }
        }

        public void Receive(IWAtSource source)
        {
            if (ReferenceEquals(source, this)) return;
            foreach (EWAt at in Enum.GetValues(typeof(EWAt)))
            {
                if (at == EWAt.Null) continue;
                BindableStat stat = GetWAt(at);
                BindableStat otherStat = source.GetWAt(at);
                if (stat != null && otherStat != null){
                    if (stat < 0)
                    {
                        // negative value means not to inherit the value from source
                        stat.Value = -stat;
                    }
                    else {
                        stat.InheritStat(otherStat);
                    }
                }
            }
        }
    }
}