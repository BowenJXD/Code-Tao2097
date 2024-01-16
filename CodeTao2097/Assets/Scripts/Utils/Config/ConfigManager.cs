using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// Manages all the config data in game
    /// </summary>
    public static class ConfigManager 
    {
        /// <summary>
        /// config datas that need to be loaded
        /// </summary>
        public static List<string> loadList = new List<string>();

        /// <summary>
        /// config datas that have been loaded
        /// </summary>
        public static Dictionary<string, ConfigData> configs = new Dictionary<string, ConfigData>();

        public static void LoadLoadList()
        {
            loadList.Clear();
            TextAsset textAsset = Resources.Load<TextAsset>(PathDefines.ConfigLoadList);
            string[] files = textAsset.text.Split('\n');
            foreach (string file in files)
            {
                loadList.Add(file.Trim());
            }
        }
        
        public static void LoadAllConfigs(bool initContent = true)
        {
            configs.Clear();
            LoadLoadList();
            foreach (string file in loadList)
            {
                ConfigData config = new ConfigData(file);
                if (initContent) {
                    TextAsset textAsset = config.LoadFile();
                    if (textAsset == null)
                    {
                        Debug.LogError($"Config file {file} not found!");
                        continue;
                    }

                    config.Load(textAsset.text);
                }
                configs.Add(file, config);
            }
        }

        public static ConfigData GetConfigData(string fileName)
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

        public static void SaveAllConfigs()
        {
            foreach (var config in configs)
            {
                string saveString = config.Value.Save();
                string path = $"{Application.dataPath}/Resources/{config.Value.fileName}.csv";
                try {
                    if (!System.IO.File.Exists(path))
                    {
                        System.IO.File.Create(path);
                    }
                    System.IO.File.WriteAllText(path, saveString);
                } catch (Exception e) {
                    Debug.LogError($"Error saving config {config.Key}: {e}");
                }
            }
        }
    }
}