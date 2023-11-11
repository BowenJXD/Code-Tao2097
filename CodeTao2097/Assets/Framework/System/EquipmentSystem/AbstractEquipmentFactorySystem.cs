using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    // 用于表现所有装备类型的枚举
    [Flags] // 可以让枚举进行多选 还需要将枚举定义成位枚举 具体的做法可以参照猫叔的写法
    public enum E_EquipType : byte
    {
        Gun = 1 << 0,       // 枪
        Sword = 1 << 1,     // 剑
        Shield = 1 << 2,    // 盾
        Helmet = 1 << 3,    // 头盔
        Armor = 1 << 4,     // 护甲
    }
    public interface IEquipConfigBuilder
    {
        List<EquipConfig> Builder();
    }
    // 用于创建子类实例的接口
    public interface IEquipConfig<T> where T : EquipInfo
    {
        T Create();
    }
    public abstract class EquipConfig
    {
        [PnShowSprite]
        public Sprite sprite;
        public string Name;
    }
    public abstract class SO_EquipConfig<T> : ScriptableObject, IEquipConfigBuilder where T : EquipConfig
    {
        [SerializeField] protected T[] configs;
        List<EquipConfig> IEquipConfigBuilder.Builder()
        {
            return configs is null || configs.Length == 0 ? null : new List<EquipConfig>(configs);
        }
    }
    [Serializable]
    public class EquipInfo
    {
        public int ID;
        public string Name;
        public E_EquipType Type;

        public EquipInfo Init(int id, string name, E_EquipType type)
        {
            ID = id;
            Name = name;
            Type = type;
            return this;
        }
    }
    public interface IEquipmentFactorySystem : ISystem
    {
        /// <summary>
        /// 获取一件新装备
        /// </summary>
        /// <param name="type">装备类别</param>
        /// <param name="id">装备ID</param>
        /// <param name="info">新装备信息</param>
        /// <returns>是否获取成功</returns>
        bool CreateEquip<T>(E_EquipType type, int id, out T info) where T : EquipInfo;
        /// <summary>
        /// 获取一件新装备
        /// </summary>
        /// <param name="type">装备类别</param>
        /// <param name="id">装备ID</param>
        /// <returns>是否获取成功</returns>
        EquipInfo CreateEquip(E_EquipType type, int id);
        /// <summary>
        /// 创建所有装备
        /// </summary>
        /// <returns>装备列表</returns>
        List<EquipInfo> CreateAllEquip();
        /// <summary>
        /// 创建一个新的蓝图
        /// </summary>
        /// <param name="type">装备的类型</param>
        /// <param name="equip">具体装备</param>
        void CreateBlueprint(E_EquipType type, EquipConfig equip);
        /// <summary>
        /// 获取装备蓝图
        /// </summary>
        /// <typeparam name="T">装备配置类</typeparam>
        /// <param name="type">需要获取的装备类型</param>
        /// <param name="id">装备ID</param>
        /// <param name="config">装备配置信息</param>
        /// <returns>是否获取成功</returns>
        bool GetBlueprint<T>(E_EquipType type, int id, out T config) where T : EquipConfig;
        /// <summary>
        /// 获取装备蓝图
        /// </summary>
        /// <typeparam name="T">装备配置类</typeparam>
        /// <param name="info">装备信息</param>
        /// <returns>装备配置</returns>
        T GetBlueprint<T>(EquipInfo info) where T : EquipConfig;
        /// <summary>
        /// 刷新工厂中的装备信息
        /// </summary>
        void RefreshFactory();
    }
    public abstract class AbstractEquipmentFactorySystem : AbstractSystem, IEquipmentFactorySystem
    {
        private Dictionary<E_EquipType, List<EquipConfig>> mEquipConfigs;
        protected abstract EquipInfo CreateRules(E_EquipType type, EquipConfig equip);
        protected override void OnInit()
        {
            RefreshFactory();
        }
        List<EquipInfo> IEquipmentFactorySystem.CreateAllEquip()
        {
            if (mEquipConfigs.Count == 0) return null;
            var infos = new List<EquipInfo>();
            foreach (var configs in mEquipConfigs)
            {
                var list = configs.Value;
                if (list == null || list.Count == 0) continue;
                for (int i = 0; i < list.Count; i++)
                {
                    var info = CreateRules(configs.Key, list[i]);
                    if (info == null) continue;
                    infos.Add(info.Init(i, list[i].Name, configs.Key));
                }
            }
            return infos;
        }
        EquipInfo IEquipmentFactorySystem.CreateEquip(E_EquipType type, int id)
        {
            if (mEquipConfigs.TryGetValue(type, out var configs))
            {
                if (id >= 0 && id < configs.Count)
                {
                    return CreateRules(type, configs[id])
                        .Init(id, configs[id].Name, type);
                }
            }
            return null;
        }
        bool IEquipmentFactorySystem.CreateEquip<T>(E_EquipType type, int id, out T info)
        {
            if (mEquipConfigs.TryGetValue(type, out var configs))
            {
                if (id >= 0 && id < configs.Count)
                {
                    info = CreateEquip<T>(configs[id]);
                    info.Init(id, configs[id].Name, type);
                    return true;
                }
            }
            info = null;
            return false;
        }
        void IEquipmentFactorySystem.CreateBlueprint(E_EquipType type, EquipConfig equip)
        {
            if (mEquipConfigs.TryGetValue(type, out var list)) list.Add(equip);
            else mEquipConfigs[type] = new List<EquipConfig>() { equip };
        }
        T IEquipmentFactorySystem.GetBlueprint<T>(EquipInfo info)
        {
            return mEquipConfigs.TryGetValue(info.Type, out var configs) ? configs[info.ID] as T : null;
        }
        bool IEquipmentFactorySystem.GetBlueprint<T>(E_EquipType type, int id, out T config)
        {
            if (mEquipConfigs.TryGetValue(type, out var configs))
            {
                if (id >= 0 && id < configs.Count)
                {
                    config = configs[id] as T;
                    return true;
                }
            }
            config = null;
            return false;
        }
        // 根据蓝图创建武器
        protected T CreateEquip<T>(EquipConfig equip) where T : EquipInfo => (equip as IEquipConfig<T>).Create();
        // 刷新工厂
        public void RefreshFactory()
        {
            if (GetOrCreateTypeConfigSO(out var configSo))
            {
                mEquipConfigs = new Dictionary<E_EquipType, List<EquipConfig>>();
                PEnum.Loop<E_EquipType>(type =>
                {
                    if ((type & configSo.types) == 0) return;
                    string soName = $"SO_{type}Config";
                    if (SerializeHelper.TryGetSO<ScriptableObject>(soName, out var so))
                    {
                        var config = so as IEquipConfigBuilder;
                        if (config == null) return;
                        var list = config.Builder();
                        if (list == null) return;
                        mEquipConfigs[type] = list;
                    }
                    else
                    {
                        SerializeHelper.ShowCreateTips<ScriptableObject>(soName);
                    }
                });
            }
        }
#if UNITY_EDITOR
        private static bool UseScriptableObject = true;
        [MenuItem("PnTool/EquipmentSystem/EnableScriptableObject")]
        public static void EnableScriptableObject()
        {
            UseScriptableObject = true;
        }
        [MenuItem("PnTool/EquipmentSystem/DisableScriptableObject")]
        public static void DisableScriptableObject()
        {
            UseScriptableObject = false;
        }
        [MenuItem("PnTool/EquipmentSystem/CreateEquipmentConfig")]
        public static void CreateEquipmentConfig()
        {
            string msg = UseScriptableObject ? "当前为ScriptableObject模式" : "当前为自定义数据模式";
            EditorUtility.DisplayDialog("喵喵提示您", msg, "OK");
            // 配置表路径
            if (GetOrCreateTypeConfigSO(out var configSo))
            {
                string soTemplate = string.Empty;
                if (UseScriptableObject)
                {
                    soTemplate = "using UnityEngine;\r\n\r\nnamespace QFramework.EquipmentSystem\r\n{\r\n    [CreateAssetMenu(fileName = \"New @Config\", menuName = \"#\")]\r\n    public class SO_@Config : SO_EquipConfig<@Config> { }\r\n}";
                }
                string infoTemplate = "namespace QFramework.EquipmentSystem\r\n{\r\n    [System.Serializable]\r\n    public class @Info : EquipInfo\r\n    {\r\n\r\n    }\r\n}";
                string configTemplate = "namespace QFramework.EquipmentSystem\r\n{\r\n    [System.Serializable]\r\n    public class @Config : EquipConfig, IEquipConfig<@Info>\r\n    {\r\n        @Info IEquipConfig<@Info>.Create()\r\n        {\r\n            throw new System.NotImplementedException();\r\n        }\r\n    }\r\n}";
                string template = "namespace QFramework.EquipmentSystem\r\n{\r\n    public class EquipmentFactorySystem : AbstractEquipmentFactorySystem\r\n    {\r\n        protected override EquipInfo CreateRules(E_EquipType type, EquipConfig equip)\r\n        {\r\n            switch (type)\r\n            {@\r\n            }\r\n            return null;\r\n        }\r\n    }\r\n}";
                string[] equipSystemSplit = template.Split('@');

                var builder = new StringBuilder(equipSystemSplit[0]);
                // 获取脚本的文件夹路径 并让他变成文件路径
                var scriptName = nameof(AbstractEquipmentFactorySystem);
                string[] paths = AssetDatabase.FindAssets(scriptName);
                // string[] paths = AssetDatabase.FindAssets(scriptName + "/EquipmentData/");
                if (paths.Length == 1)
                {
                    string path = AssetDatabase.GUIDToAssetPath(paths[0]).Replace($@"/{scriptName}.cs", "");
                    path += "/EquipmentData/";
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    PEnum.Loop<E_EquipType>(type =>
                    {
                        if ((type & configSo.types) == 0) return;
                        string key = type.ToString();
                        if (UseScriptableObject)
                        {
                            SerializeHelper.WriteOnlyAtCreate(path + $"SO_{key}Config.cs", soTemplate.Replace("@", key).Replace("#", $"Data/SO/{key}Config"));
                        }
                        SerializeHelper.WriteOnlyAtCreate(path + $"{key}Config.cs", configTemplate.Replace("@", key));
                        SerializeHelper.WriteOnlyAtCreate(path + $"{key}Info.cs", infoTemplate.Replace("@", key));
                        builder.Append($"\r\n\t\t\t\tcase E_EquipType.{key}: return CreateEquip<{key}Info>(equip);");
                    });
                    builder.Append(equipSystemSplit[1]);
                    // 将文件写入脚本
                    SerializeHelper.WriteOnlyAtCreate(path + "EquipmentFactorySystem.cs", builder.ToString());
                    AssetDatabase.Refresh();
                }
                else EditorUtility.DisplayDialog("喵喵提示您", "路径错误", "OK");
            }
        }
        private static bool GetOrCreateTypeConfigSO(out SO_EquipTypeConfig configSo)
        {
            if (SerializeHelper.TryGetSO("EquipTypeConfig", out configSo)) return true;
            SerializeHelper.ShowCreateTips<SO_EquipTypeConfig>("EquipTypeConfig");
            return false;
        }
#endif
    }
}