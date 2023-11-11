using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class CommonData
    {
        public static readonly string StoragePath = Application.streamingAssetsPath + "/Storage/";
        public static readonly string ConfigPath = Application.streamingAssetsPath + "/Config/";
    }
    public class SerializeHelper
    {
        /// <summary>
        /// 尝试获取So 如果没有 就创建一个
        /// </summary>
        /// <returns>如果获取成功 返回True</returns>
        public static bool TryGetSO<T>(string soName, out T so, bool isCreate = true) where T : ScriptableObject
        {
            string soPath = "Assets/Resources/SO_Data/";
            // 如果文件夹存在 有可能存在文件不存在的情况 加载一次看看
            if (Directory.Exists(soPath))
            {
                // 如果文件存在 直接加载
                so = Resources.Load<T>("SO_Data/" + soName);
                // 如果文件不存在 判断是否创建 
                if (so != null) return true;
                if (isCreate) so = CreateSO<T>(soName, soPath);
            }
            else
            {
                so = null;
                Directory.CreateDirectory(soPath);
                if (isCreate) so = CreateSO<T>(soName, soPath);
            }
            return false;
        }
        public static T CreateSO<T>(string soName, string soPath) where T : ScriptableObject
        {
            // 如果文件不存在 创建So文件
            T so = ScriptableObject.CreateInstance<T>();
            // 拼接文本 补全路径 写入本地
            AssetDatabase.CreateAsset(so, soPath + $"{soName}.asset");
            // 刷新文件夹
            AssetDatabase.Refresh();
            return so;
        }
        // 仅在创建时写入
        public static void WriteOnlyAtCreate(string filePath, string msg)
        {
            // 检验一个路径的合法性 如果文件名为空 表示目录 否则表示文件
            if (string.IsNullOrEmpty(Path.GetFileName(filePath))) throw new Exception("未找到文件");
            if (File.Exists(filePath)) return;
            // 否则创建并写入
            using (FileStream fs = File.Create(filePath))
            {
                byte[] buff = Encoding.Default.GetBytes(msg);
                fs.Write(buff, 0, buff.Length);
            }
        }
        public static void ShowCreateTips<T>(string soName)
        {
            string msg = $"{soName}已在Res目录下创建{typeof(T)},请前往查看或配置";
            EditorUtility.DisplayDialog("喵喵提示您", msg, "OK");
        }
        public static T LoadJson<T>(string path)
        {
            // 如果文件存在就加载
            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                if (bytes.Length > 0)
                {
                    string json = Encoding.UTF8.GetString(bytes);
                    return JsonUtility.FromJson<T>(json);
                }
            }
            return default;
        }
        public static void SaveJson<T>(string path, T data)
        {
            var file = new FileInfo(path);
            using (FileStream fs = file.Exists ? file.OpenWrite() : file.Create())
            {
                string json = JsonUtility.ToJson(data);
                byte[] buff = Encoding.UTF8.GetBytes(json);
                fs.Write(buff, 0, buff.Length);
            }
        }
        public static T LoadBinary<T>(string path)
        {
            // 如果文件存在
            if (File.Exists(path))
            {
                //打开一个文件流
                using (FileStream fs = File.Open(path, FileMode.Open))
                {
                    //反序列化方法
                    return (T)new BinaryFormatter().Deserialize(fs);
                }
            }
            return default;
        }
        public static void SaveBinary<T>(string path, T data)
        {
            var file = new FileInfo(path);
            // 如果当前目录存在该文件 打开并写入文件 否则 创建存档文件
            using (FileStream fs = file.Exists ? file.OpenWrite() : file.Create())
            {
                // 将对象序列化为二进制
                new BinaryFormatter().Serialize(fs, data);
            }
        }
    }
}