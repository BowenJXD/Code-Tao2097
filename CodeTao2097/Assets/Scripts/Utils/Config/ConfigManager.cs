using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// Manages all the config data in game
    /// </summary>
    public class ConfigManager : MonoSingleton<ConfigManager>
    {
        /// <summary>
        /// config datas that need to be loaded
        /// </summary>
        protected Dictionary<string, ConfigData> loadList;

        /// <summary>
        /// config datas that have been loaded
        /// </summary>
        protected Dictionary<string, ConfigData> configs;
        
        void Start()
        {
            loadList = new Dictionary<string, ConfigData>();
            configs = new Dictionary<string, ConfigData>();
        }

        /// <summary>
        /// Register the config data needed to be loaded
        /// </summary>
        /// <param name="file"></param>
        /// <param name="config"></param>
        public void Register(string file, ConfigData config)
        {
            loadList[file] = config;
        }

        public void LoadAllConfigs()
        {
            foreach (var item in loadList)
            {
                TextAsset textAsset = item.Value.LoadFile();
                item.Value.Load(textAsset.text);
                configs.Add(item.Value.fileName, item.Value);
            }
            loadList.Clear();
        }

        public ConfigData GetConfigData(string fileName)
        {
            if (configs.ContainsKey(fileName))
            {
                return configs[fileName];
            }
            else
            {
                Debug.LogError($"Config file {fileName} not found!");
                return null;
            }
        }
    }
}