using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public interface IAudioMgrSystem : ISystem
    {
        void PlayBgm(string name);
        void StopBgm(bool isPause = true);
        void PlaySound(string name);
        void RegisterLoopSound(GameObject obj, string name, Func<bool> onPlay, Func<bool> onStop);
        void RecoverySound(GameObject obj);

        IBindableProperty<float> BgmVolume { get; }
        IBindableProperty<float> SoundVolume { get; }
    }
    /// <summary>
    /// 实现音频池 和 音效组件管理
    /// </summary>
    public class AudioMgrSystem : AbstractSystem, IAudioMgrSystem
    {
        private class RecoveryItem
        {
            private AudioSource mSource;
            public AudioSource Source => mSource;

            private Func<bool> onPlay, onStop;

            public RecoveryItem(Func<bool> onPlay, Func<bool> onStop, AudioSource source)
            {
                this.onPlay = onPlay;
                this.onStop = onStop;
                this.mSource = source;
            }
            public void Check()
            {
                if (onPlay() && !mSource.isPlaying)
                {
                    mSource.Play();
                }
                else if (onStop() && mSource.isPlaying)
                {
                    mSource.Stop();
                }
            }
            public void Recovery()
            {
                mSource.Stop();
                mSource.enabled = false;
            }
        }

        public IBindableProperty<float> BgmVolume { get; } = new BindableProperty<float>(1);
        public IBindableProperty<float> SoundVolume { get; } = new BindableProperty<float>(1);

        /// <summary>
        /// 背景音乐播放组件
        /// </summary>
        private AudioSource mBGM;
        /// <summary>
        /// 音量渐变工具
        /// </summary>
        private FadeNum mFade;
        /// <summary>
        /// 组件根对象
        /// </summary>
        private GameObject mRoot;
        /// <summary>
        /// 循环音频回收对象组
        /// </summary>
        private Dictionary<GameObject, List<RecoveryItem>> mRecoveryItems;
        /// <summary>
        /// 存储所有已激活组件
        /// </summary>
        private LinkedList<AudioSource> mOpenList;
        /// <summary>
        /// 存储所有未使用组件
        /// </summary>
        private Queue<LinkedListNode<AudioSource>> mCloseList;
        /// <summary>
        /// 初始化系统
        /// </summary>
        protected override void OnInit()
        {
            mOpenList = new LinkedList<AudioSource>();
            mCloseList = new Queue<LinkedListNode<AudioSource>>();
            mRecoveryItems = new Dictionary<GameObject, List<RecoveryItem>>();

            mFade = new FadeNum();
            mFade.SetMinMax(0, BgmVolume.Value);

            BgmVolume.RegisterWithInitValue(OnBgmVolumeChanged);
            SoundVolume.RegisterWithInitValue(OnSoundVolumeChanged);

            PublicMono.Instance.OnUpdate += Update;
        }
        // 初始化根节点
        private void InitRootObject()
        {
            if (mRoot == null)
            {
                mRoot = new GameObject("AudioSourcePool");
                GameObject.DontDestroyOnLoad(mRoot);
            }
        }
        /// <summary>
        /// 更新音量
        /// </summary>
        private void Update()
        {
            foreach (var list in mRecoveryItems.Values)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Check();
                }
            }
            if (mFade.IsFinish) return;
            mBGM.volume = mFade.Update(Time.deltaTime);
        }
        /// <summary>
        /// 当背景音乐音量改变时
        /// </summary>
        private void OnBgmVolumeChanged(float v)
        {
            mFade.SetMinMax(0f, v);

            if (mBGM == null) return;

            mBGM.volume = v;
        }
        /// <summary>
        /// 当音效音量被改变
        /// </summary>
        /// <param name="v">音量</param>
        private void OnSoundVolumeChanged(float v)
        {
            //当有音效在播 调节所有在播音效音量
            foreach (var source in mOpenList) source.volume = v;
        }
        /// <summary>
        /// 自动回收组件
        /// </summary>
        /// <param name="condition">回收条件</param>
        public void AutoPush()
        {
            // 可能开启列表有东西可以回收 看看那些满足条件的组件 就把他回收掉
            if (mOpenList.Count == 0) return;
            // 拿到链表头部节点
            var currentNode = mOpenList.First;
            // 如果拿到当前组件
            while (currentNode != null)
            {
                // 获取当前组件
                var source = currentNode.Value;
                //如果没有在播放 就回收组件
                if (!source.isPlaying)
                {
                    source.enabled = false;
                    mCloseList.Enqueue(currentNode);
                    mOpenList.Remove(currentNode);
                }
                // 将下一个节点 赋值给 当前节点
                currentNode = currentNode.Next;
            }
        }
        /// <summary>
        /// 播放音效
        /// </summary>
        void IAudioMgrSystem.PlaySound(string name)
        {
            //先自动回收 音频源组件
            AutoPush();
            //如果关闭列表没有东西的时候  可能是第一次使用
            AudioSource tempSource = null;
            if (mCloseList.Count == 0)
            {
                InitRootObject();
                tempSource = mRoot.AddComponent<AudioSource>();
                mOpenList.AddLast(tempSource);
            }
            else // 如果关闭列表有东西 启用一个组件
            {
                var node = mCloseList.Dequeue();
                //获取一个未使用组件并激活组件
                tempSource = node.Value;
                tempSource.enabled = true;
                //把组件节点 放入开启列表
                mOpenList.AddLast(node);
            }
            //加载一个音效
            ResHelper.AsyncLoad<AudioClip>("Audio/Sound/" + name, clip =>
            {
                tempSource.clip = clip;
                tempSource.volume = SoundVolume.Value;
                tempSource.loop = false;
                tempSource.Play();
            });
        }
        void IAudioMgrSystem.RegisterLoopSound(GameObject obj, string name, Func<bool> onPlay, Func<bool> onStop)
        {
            InitRootObject();
            var source = mRoot.AddComponent<AudioSource>();
            //加载一个音效
            ResHelper.AsyncLoad<AudioClip>("Audio/Sound/" + name, clip =>
            {
                source.clip = clip;
                source.volume = SoundVolume.Value;
                source.loop = true;
            });
            var item = new RecoveryItem(onPlay, onStop, source);
            if (mRecoveryItems.TryGetValue(obj, out var list)) list.Add(item);
            else mRecoveryItems.Add(obj, new List<RecoveryItem>() { item });
        }
        void IAudioMgrSystem.RecoverySound(GameObject obj)
        {
            if (mRecoveryItems.TryGetValue(obj, out var list))
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    list[i].Recovery();
                    GameObject.Destroy(list[i].Source);
                    list.RemoveAt(i);
                }
                mRecoveryItems.Remove(obj);
            }
        }
        /// <summary>
        /// 停止背景音乐
        /// </summary>
        void IAudioMgrSystem.StopBgm(bool isPause)
        {
            if (mBGM == null) return;
            if (mBGM.isPlaying)
            {
                mFade.SetState(FadeState.FadeOut, () =>
                {
                    if (isPause) mBGM.Pause();
                    else mBGM.Stop();
                });
            }
        }
        /// <summary>
        /// 播放BGM
        /// </summary>
        /// <param name="name">音乐资源名字</param>
        void IAudioMgrSystem.PlayBgm(string name)
        {
            if (mBGM == null)
            {
                var o = new GameObject("GameBGM");
                GameObject.DontDestroyOnLoad(o);
                mBGM = o.AddComponent<AudioSource>();
                mBGM.loop = true;
                mBGM.volume = 0;
            }
            //加载一个音效
            ResHelper.AsyncLoad<AudioClip>("Audio/Bgm/" + name, clip =>
            {
                //如果有bgm正在播放 就先把音量降下来 1 - 0 再播放当前音乐 0 - 1                
                if (mBGM.isPlaying)
                {
                    mFade.SetState(FadeState.FadeOut, () => Play(clip));
                }
                //如果没有东西在播放
                else Play(clip);
            });
        }
        private void Play(AudioClip clip)
        {
            mFade.SetState(FadeState.FadeIn);
            mBGM.clip = clip;
            mBGM.Play();
        }
    }
}