using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 单次伤害的信息，包括伤害的时间，玩家等级，伤害者等级，以及伤害相关的属性。
    /// </summary>
    public class DamageLog
    {
        public Damage damage;
        public float time;
        public int playerLevel;
        public int damagerLevel;
        
        public DamageLog(Damage damage, float time, int playerLevel, int damagerLevel)
        {
            this.damage = damage;
            this.time = time;
            this.playerLevel = playerLevel;
            this.damagerLevel = damagerLevel;
        }
        
        public static string GetCSVHeader()
        {
            return $"Time,{Damage.GetCSVHeader()},PlayerLevel,DamagerLevel";
        }
        
        public string ToCSV()
        {
            return $"{time},{damage.ToCSV()},{playerLevel},{damagerLevel}";
        }
    }

    /// <summary>
    /// 单个damager在每个玩家等级下的伤害统计，包括总伤害，总DPS，以及每个玩家等级下的DPS。
    /// </summary>
    public class DamagerPlayerLevelStat
    {
        public Damager damager;
        public List<float> playerLevelDamage = new List<float>();
        
        public DamagerPlayerLevelStat(Damager damager)
        {
            this.damager = damager;
        }

        public void AddLevelDamage(int level, float damage)
        {
            if (playerLevelDamage.Count <= level)
            {
                playerLevelDamage.Add(damage);
            }
            else
            {
                playerLevelDamage[level] += damage;
            }
        }
        
        public static string GetCSVHeader(int playerLevelCount)
        {
            string result = "Damager,TotalDamage,TotalDps,";
            for (int i = 0; i < playerLevelCount; i++)
            {
                result += $"Level{i},";
            }
            return result.TrimEnd(',') + '\n';
        }
        
        public string ToCSV(List<float> playerLevelStartTime)
        {
            float totalDamage = playerLevelDamage.Sum();
            float totalDps = totalDamage / (playerLevelStartTime[playerLevelStartTime.Count - 1] - playerLevelStartTime[0]);
            string result = $"{damager.name},{totalDamage},{totalDps},";
            for (int i = 0; i < playerLevelDamage.Count; i++)
            {
                if (i + 1 >= playerLevelDamage.Count && i + 1 >= playerLevelStartTime.Count) break;
                result += $"{playerLevelDamage[i] / (playerLevelStartTime[i + 1] - playerLevelStartTime[i])},";
            }
            return result.TrimEnd(',');
        }
    }
    
    /// <summary>
    /// 单个damager在每个等级下的伤害统计，包括总伤害，总DPS，以及每个等级下的DPS。
    /// </summary>
    public class DamagerLevelStat
    {
        public Damager damager;
        public List<float> damagerLevelDamage = new List<float>();
        
        public DamagerLevelStat(Damager damager)
        {
            this.damager = damager;
        }

        public void AddLevelDamage(int level, float damage)
        {
            if (damagerLevelDamage.Count <= level)
            {
                damagerLevelDamage.Add(damage);
            }
            else
            {
                damagerLevelDamage[level] += damage;
            }
        }
        
        /// <summary>
        /// 10 levels
        /// </summary>
        /// <returns></returns>
        public static  string GetCSVHeader()
        {
            string result = "Damager,TotalDamage,TotalDps,";
            for (int i = 0; i < 10; i++)
            {
                result += $"Level{i},";
            }

            return result.TrimEnd(',') + '\n';
        }
        
        /// <summary>
        /// Returns a list of dps for each level
        /// </summary>
        /// <param name="damagerLevelStartTime"></param>
        /// <returns></returns>
        public string ToCSV(List<float> damagerLevelStartTime)
        {
            float totalDamage = damagerLevelDamage.Sum();
            float totalDps = totalDamage / (damagerLevelStartTime[damagerLevelStartTime.Count - 1] - damagerLevelStartTime[0]);
            string result = $"{damager.name},{totalDamage},{totalDps}";
            for (int i = 0; i < damagerLevelDamage.Count; i++)
            {
                if (i + 1 >= damagerLevelDamage.Count && i + 1 >= damagerLevelStartTime.Count) break;
                result += $"{damagerLevelDamage[i] / (damagerLevelStartTime[i + 1] - damagerLevelStartTime[i])},";
            }
            return result;
        }
    }
    
    /// <summary>
    /// 伤害统计工具，负责记录和导出三种伤害信息文件（Resources/DamageStatistics/）。
    /// </summary>
    public class DamageStatistics : MonoSingleton<DamageStatistics>
    {
        protected List<DamageLog> damageLogs = new List<DamageLog>();
        protected Dictionary<Damager, DamagerPlayerLevelStat> damagerPlayerLevelStats = new Dictionary<Damager, DamagerPlayerLevelStat>();
        protected Dictionary<Damager, DamagerLevelStat> damagerLevelStats = new Dictionary<Damager, DamagerLevelStat>();
        protected List<float> playerLevelStartTimes = new List<float>{0};
        protected Dictionary<Damager, List<float>> damagerLevelStartTimes = new Dictionary<Damager, List<float>>();
        protected List<Damager> damagers = new List<Damager>();

        protected ExpController playerExpController;

        private void Start()
        {
            DamageManager.Instance.damageAfter += AddDamage;
            playerExpController = Player.Instance.GetComponentFromUnit<ExpController>();
            playerExpController.LVL.RegisterWithInitValue(level =>
            {
                playerLevelStartTimes.Add(Global.GameTime);
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        void AddDamage(Damage damage)
        {
            float time = Global.GameTime;
            int playerLevel = playerExpController.LVL;
            Item damagerItem = damage.Median.GetComponentInAncestors<Item>();
            int damagerLevel = damagerItem ? damagerItem.LVL : 0;
            Damager damager = damage.Median;
            float damageValue = damage.GetDamageValue();
            
            damageLogs.Add(new DamageLog(damage, time, playerLevel, damagerLevel));

            if (!damagers.Contains(damager))
            {
                damagers.Add(damager);
                damagerPlayerLevelStats.Add(damager, new DamagerPlayerLevelStat(damager));
                damagerLevelStats.Add(damager, new DamagerLevelStat(damager));
                damagerLevelStartTimes.Add(damager, new List<float>{0});
                damagerItem?.LVL.RegisterWithInitValue(level =>
                {
                    damagerLevelStartTimes[damager].Add(Global.GameTime);
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
            else
            {
                damagerPlayerLevelStats[damager].AddLevelDamage(playerLevel, damageValue);
                damagerLevelStats[damager].AddLevelDamage(damagerLevel, damageValue);
            }
        }
        
        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            
            playerLevelStartTimes.Add(Global.GameTime);
            foreach (var kvp in damagerLevelStartTimes)
            {
                kvp.Value.Add(Global.GameTime);
            }

            SaveAsCSV();
        }
        
        void SaveAsCSV()
        {
            string directoryPath = Application.dataPath + "/Resources/DamageStatistics/";
            
            // Save damage statistics
            string damageStatistics = DamageLog.GetCSVHeader() + "\n";
            foreach (DamageLog damage in damageLogs)
            {
                damageStatistics += damage.ToCSV() + "\n";
            }
            WriteToCSV(directoryPath + "DamageStatistics.csv", damageStatistics);
            
            // Save damager player level statistics
            string damagerPlayerLevelStatistics = DamagerPlayerLevelStat.GetCSVHeader(playerLevelStartTimes.Count);
            foreach (var damagerPlayerLevelStat in damagerPlayerLevelStats.Values)
            {
                damagerPlayerLevelStatistics += damagerPlayerLevelStat.ToCSV(playerLevelStartTimes) + "\n";
            }
            WriteToCSV(directoryPath + "DamagerPlayerLevelStatistics.csv", damagerPlayerLevelStatistics);
            
            // Save damager level statistics, 10 levels
            string damagerLevelStatistics = DamagerLevelStat.GetCSVHeader();
            foreach (var damagerLevelStat in damagerLevelStats.Values)
            {
                damagerLevelStatistics += damagerLevelStat.ToCSV(damagerLevelStartTimes[damagerLevelStat.damager]) + "\n";
            }
            WriteToCSV(directoryPath + "DamagerLevelStatistics.csv", damagerLevelStatistics);
        }
        
        void WriteToCSV(string path, string content)
        {
            System.IO.File.WriteAllText(path, content);
        }
    }
}