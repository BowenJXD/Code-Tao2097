using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// �浵���ݽӿ�
    /// </summary>
    public interface IArchive { }
    /// <summary>
    /// ��ǽӿ�
    /// </summary>
    public interface IMark { }
    /// <summary>
    /// �浵������ �ǵ�ע���Ƕ���
    /// </summary>
    /// <typeparam name="T">�浵��������</typeparam>
    public interface ISaveMark<T> : IMark where T : IArchive
    {
        void Save(T archive);
        void Load(T archive);
    }
    /// <summary>
    /// ϵͳ�ӿ� ʹ�ñ�ǽ��д浵ʱ��Ҫע���Լ�
    /// </summary>
    public interface IArchiveSystem : ISystem
    {
        /// <summary>
        /// �����ݴ洢���ļ�����
        /// </summary>
        /// <typeparam name="T">�浵��������</typeparam>
        /// <param name="id">��ID Ϊ ����ʱ��ʾ���������л����� ֻ��������</param>
        void Save<T>(int id = -1) where T : class, IArchive, new();
        /// <summary>
        /// �����ļ��еĴ浵�ļ�
        /// </summary>
        /// <typeparam name="T">�浵��������</typeparam>
        /// <param name="id">��ID Ϊ ����ʱ��ʾ���¼��ػ�������</param>
        bool Load<T>(int id = -1) where T : class, IArchive;
        /// <summary>
        /// ��Ӵ浵��Ƕ���
        /// </summary>
        bool Add<T>(ISaveMark<T> mark) where T : IArchive;
        /// <summary>
        /// �Ƴ��浵��Ƕ���
        /// </summary>
        bool Remove<T>(ISaveMark<T> mark) where T : IArchive;
    }
    public struct GameSaveEvent<T> where T : IArchive { public T data; } // �浵�¼�
    public struct GameLoadEvent<T> where T : IArchive { public T data; } // �����¼�
    /// <summary>
    /// �浵ϵͳ
    /// </summary>
    public class ArchiveSystem : AbstractSystem, IArchiveSystem
    {
        private Dictionary<string, IArchive> mArchives;         // ��ǰ�������͵Ĵ浵
        private Dictionary<string, HashSet<IMark>> mMarkInfos;  // ��Ŀ���д浵���
        private HashSet<string> mDiertys;                       // ���д浵����

        protected override void OnInit()
        {
            mDiertys = new HashSet<string>();
            mMarkInfos = new Dictionary<string, HashSet<IMark>>();
            mArchives = new Dictionary<string, IArchive>();
        }
        private void Send<T>(T data) where T : IArchive
        {
            // �����ݷ��͸���������
            this.SendEvent(new GameLoadEvent<T>() { data = data });
            // �������б��Ԫ�� �� Load ����
            if (mMarkInfos.TryGetValue(typeof(T).ToString(), out var marks))
            {
                foreach (var mark in marks) (mark as ISaveMark<T>).Load(data);
            }
        }
        bool IArchiveSystem.Load<T>(int id)
        {
            string name = $"{typeof(T)}_{id}";
            // ���ID��Ч ���Ի�ȡ�����ļ�
            if (id >= 0)
            {
                // ������ڱ�� ˵�������е�����Ϊ���� ������Ϊ�������ݻ����ݲ�������
                if (mDiertys.Contains(name))
                {
                    if (mArchives.TryGetValue(name, out var data))
                    {
                        Send<T>(data as T);
                        Debug.Log("��ȡ����");
                        return true;
                    }
                }
                // �ӱ����ҵ��浵���� ���浵���ݸ�ֵ������
                else if (this.GetUtility<IStorage>().Load<T>(name, out T archive))
                {
                    mDiertys.Add(name);
                    mArchives[name] = archive;
                    Send<T>(archive);
                    Debug.Log("��ȡIO����");
                    return true;
                }
            }
            // ���浵IDΪ-1 ���ж��Ƿ��Ѵ��ڻ��� �����ش浵
            else if (mArchives.TryGetValue(name, out var data))
            {
                Send<T>(data as T);
                return true;
            }
            return false;
        }
        // ���� => ��ʱ�浵 => ���س־û�
        void IArchiveSystem.Save<T>(int id)
        {
            string type = typeof(T).ToString();
            // ��ȡ��Ҫ��������������
            string name = $"{type}_{id}";
            T data = default;
            // ������ڸ����� �����ݽ��л���
            if (mArchives.ContainsKey(name)) data = (T)mArchives[name];
            // ��������ڸ����� �ͳ�ʼ��һ������������ �� ��ӵ��ֵ���
            else mArchives[name] = data = new T();
            // �����浵�¼� ���浱ǰ�浵������ ���ʹ浵�¼�
            this.SendEvent(new GameSaveEvent<T>() { data = data });
            // ִ�����д浵��Ƕ���� Save ����
            if (mMarkInfos.TryGetValue(type, out var marks))
            {
                foreach (var mark in marks) (mark as ISaveMark<T>).Save(data);
            }
            // ����浵 ID ��Ч ��������л�����
            if (id >= 0)
            {
                this.GetUtility<IStorage>().Save<T>(name, data);
                mDiertys.Remove(name); // �����Ƴ���� 
            }
        }
        bool IArchiveSystem.Add<T>(ISaveMark<T> mark)
        {
            string type = typeof(T).ToString();
            // ������ڸ����� ��������������µı��
            if (mMarkInfos.TryGetValue(type, out var marks)) return marks.Add(mark);
            // ��������� �򴴽�һ������� �� ����±��
            mMarkInfos[type] = new HashSet<IMark> { mark };
            return true;
        }
        bool IArchiveSystem.Remove<T>(ISaveMark<T> mark)
        {
            string type = typeof(T).ToString();
            // ������ڸ����� ˵�����ܴ��ڱ�� �����Ƴ�
            if (mMarkInfos.TryGetValue(type, out var marks)) return marks.Remove(mark);
            // ��������� ˵���϶�û�б�� ֱ������
            return false;
        }
    }
}