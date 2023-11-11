using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 存档数据接口
    /// </summary>
    public interface IArchive { }
    /// <summary>
    /// 标记接口
    /// </summary>
    public interface IMark { }
    /// <summary>
    /// 存档对象标记 记得注册标记对象
    /// </summary>
    /// <typeparam name="T">存档数据类型</typeparam>
    public interface ISaveMark<T> : IMark where T : IArchive
    {
        void Save(T archive);
        void Load(T archive);
    }
    /// <summary>
    /// 系统接口 使用标记进行存档时需要注册自己
    /// </summary>
    public interface IArchiveSystem : ISystem
    {
        /// <summary>
        /// 将数据存储到文件夹中
        /// </summary>
        /// <typeparam name="T">存档数据类型</typeparam>
        /// <param name="id">当ID 为 负数时表示不进行序列化操作 只缓存数据</param>
        void Save<T>(int id = -1) where T : class, IArchive, new();
        /// <summary>
        /// 加载文件中的存档文件
        /// </summary>
        /// <typeparam name="T">存档数据类型</typeparam>
        /// <param name="id">当ID 为 负数时表示重新加载缓存数据</param>
        bool Load<T>(int id = -1) where T : class, IArchive;
        /// <summary>
        /// 添加存档标记对象
        /// </summary>
        bool Add<T>(ISaveMark<T> mark) where T : IArchive;
        /// <summary>
        /// 移除存档标记对象
        /// </summary>
        bool Remove<T>(ISaveMark<T> mark) where T : IArchive;
    }
    public struct GameSaveEvent<T> where T : IArchive { public T data; } // 存档事件
    public struct GameLoadEvent<T> where T : IArchive { public T data; } // 加载事件
    /// <summary>
    /// 存档系统
    /// </summary>
    public class ArchiveSystem : AbstractSystem, IArchiveSystem
    {
        private Dictionary<string, IArchive> mArchives;         // 当前所有类型的存档
        private Dictionary<string, HashSet<IMark>> mMarkInfos;  // 项目所有存档标记
        private HashSet<string> mDiertys;                       // 所有存档脏标记

        protected override void OnInit()
        {
            mDiertys = new HashSet<string>();
            mMarkInfos = new Dictionary<string, HashSet<IMark>>();
            mArchives = new Dictionary<string, IArchive>();
        }
        private void Send<T>(T data) where T : IArchive
        {
            // 将数据发送给各个对象
            this.SendEvent(new GameLoadEvent<T>() { data = data });
            // 遍历所有标记元素 的 Load 方法
            if (mMarkInfos.TryGetValue(typeof(T).ToString(), out var marks))
            {
                foreach (var mark in marks) (mark as ISaveMark<T>).Load(data);
            }
        }
        bool IArchiveSystem.Load<T>(int id)
        {
            string name = $"{typeof(T)}_{id}";
            // 如果ID有效 则尝试获取本地文件
            if (id >= 0)
            {
                // 如果存在标记 说明缓存中的数据为最新 代表缓存为中无数据或数据不是最新
                if (mDiertys.Contains(name))
                {
                    if (mArchives.TryGetValue(name, out var data))
                    {
                        Send<T>(data as T);
                        Debug.Log("获取缓存");
                        return true;
                    }
                }
                // 从本地找到存档数据 将存档数据赋值给缓存
                else if (this.GetUtility<IStorage>().Load<T>(name, out T archive))
                {
                    mDiertys.Add(name);
                    mArchives[name] = archive;
                    Send<T>(archive);
                    Debug.Log("获取IO数据");
                    return true;
                }
            }
            // 当存档ID为-1 先判断是否已存在缓存 再重载存档
            else if (mArchives.TryGetValue(name, out var data))
            {
                Send<T>(data as T);
                return true;
            }
            return false;
        }
        // 数据 => 临时存档 => 本地持久化
        void IArchiveSystem.Save<T>(int id)
        {
            string type = typeof(T).ToString();
            // 获取需要读档的数据类型
            string name = $"{type}_{id}";
            T data = default;
            // 如果存在该数据 将数据进行缓存
            if (mArchives.ContainsKey(name)) data = (T)mArchives[name];
            // 如果不存在该类型 就初始化一个该类型数据 并 添加到字典中
            else mArchives[name] = data = new T();
            // 构建存档事件 缓存当前存档的引用 发送存档事件
            this.SendEvent(new GameSaveEvent<T>() { data = data });
            // 执行所有存档标记对象的 Save 方法
            if (mMarkInfos.TryGetValue(type, out var marks))
            {
                foreach (var mark in marks) (mark as ISaveMark<T>).Save(data);
            }
            // 如果存档 ID 有效 则进行序列化操作
            if (id >= 0)
            {
                this.GetUtility<IStorage>().Save<T>(name, data);
                mDiertys.Remove(name); // 尝试移除标记 
            }
        }
        bool IArchiveSystem.Add<T>(ISaveMark<T> mark)
        {
            string type = typeof(T).ToString();
            // 如果存在该类型 尝试向里面添加新的标记
            if (mMarkInfos.TryGetValue(type, out var marks)) return marks.Add(mark);
            // 如果不存在 则创建一个标记组 并 添加新标记
            mMarkInfos[type] = new HashSet<IMark> { mark };
            return true;
        }
        bool IArchiveSystem.Remove<T>(ISaveMark<T> mark)
        {
            string type = typeof(T).ToString();
            // 如果存在该类型 说明可能存在标记 尝试移除
            if (mMarkInfos.TryGetValue(type, out var marks)) return marks.Remove(mark);
            // 如果不存在 说明肯定没有标记 直接跳出
            return false;
        }
    }
}