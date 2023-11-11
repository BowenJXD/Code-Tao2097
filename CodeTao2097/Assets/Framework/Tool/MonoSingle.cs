using System;
using UnityEngine;

namespace QFramework
{
    // 脚本单例
    public abstract class MonoSingle<T> : MonoBehaviour where T : Component
    {
        private static T mInstance;
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new Lazy<GameObject>().Value.AddComponent<T>();
                    mInstance.gameObject.name = typeof(T).Name;
                }
                return mInstance;
            }
        }
        protected virtual void Awake()
        {
            if (mInstance != null) GameObject.Destroy(gameObject);
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }
    // 单例接口
    public interface ISingleton { void Init(); }
    // 抽象单例基类
    public abstract class Singleton<T> where T : ISingleton
    {
        private static T mInstance;
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new Lazy<T>(true).Value;
                    mInstance.Init();
                }
                return mInstance;
            }
        }
    }
}