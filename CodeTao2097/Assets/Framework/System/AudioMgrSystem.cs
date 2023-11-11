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
    /// ʵ����Ƶ�� �� ��Ч�������
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
        /// �������ֲ������
        /// </summary>
        private AudioSource mBGM;
        /// <summary>
        /// �������乤��
        /// </summary>
        private FadeNum mFade;
        /// <summary>
        /// ���������
        /// </summary>
        private GameObject mRoot;
        /// <summary>
        /// ѭ����Ƶ���ն�����
        /// </summary>
        private Dictionary<GameObject, List<RecoveryItem>> mRecoveryItems;
        /// <summary>
        /// �洢�����Ѽ������
        /// </summary>
        private LinkedList<AudioSource> mOpenList;
        /// <summary>
        /// �洢����δʹ�����
        /// </summary>
        private Queue<LinkedListNode<AudioSource>> mCloseList;
        /// <summary>
        /// ��ʼ��ϵͳ
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
        // ��ʼ�����ڵ�
        private void InitRootObject()
        {
            if (mRoot == null)
            {
                mRoot = new GameObject("AudioSourcePool");
                GameObject.DontDestroyOnLoad(mRoot);
            }
        }
        /// <summary>
        /// ��������
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
        /// ���������������ı�ʱ
        /// </summary>
        private void OnBgmVolumeChanged(float v)
        {
            mFade.SetMinMax(0f, v);

            if (mBGM == null) return;

            mBGM.volume = v;
        }
        /// <summary>
        /// ����Ч�������ı�
        /// </summary>
        /// <param name="v">����</param>
        private void OnSoundVolumeChanged(float v)
        {
            //������Ч�ڲ� ���������ڲ���Ч����
            foreach (var source in mOpenList) source.volume = v;
        }
        /// <summary>
        /// �Զ��������
        /// </summary>
        /// <param name="condition">��������</param>
        public void AutoPush()
        {
            // ���ܿ����б��ж������Ի��� ������Щ������������� �Ͱ������յ�
            if (mOpenList.Count == 0) return;
            // �õ�����ͷ���ڵ�
            var currentNode = mOpenList.First;
            // ����õ���ǰ���
            while (currentNode != null)
            {
                // ��ȡ��ǰ���
                var source = currentNode.Value;
                //���û���ڲ��� �ͻ������
                if (!source.isPlaying)
                {
                    source.enabled = false;
                    mCloseList.Enqueue(currentNode);
                    mOpenList.Remove(currentNode);
                }
                // ����һ���ڵ� ��ֵ�� ��ǰ�ڵ�
                currentNode = currentNode.Next;
            }
        }
        /// <summary>
        /// ������Ч
        /// </summary>
        void IAudioMgrSystem.PlaySound(string name)
        {
            //���Զ����� ��ƵԴ���
            AutoPush();
            //����ر��б�û�ж�����ʱ��  �����ǵ�һ��ʹ��
            AudioSource tempSource = null;
            if (mCloseList.Count == 0)
            {
                InitRootObject();
                tempSource = mRoot.AddComponent<AudioSource>();
                mOpenList.AddLast(tempSource);
            }
            else // ����ر��б��ж��� ����һ�����
            {
                var node = mCloseList.Dequeue();
                //��ȡһ��δʹ��������������
                tempSource = node.Value;
                tempSource.enabled = true;
                //������ڵ� ���뿪���б�
                mOpenList.AddLast(node);
            }
            //����һ����Ч
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
            //����һ����Ч
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
        /// ֹͣ��������
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
        /// ����BGM
        /// </summary>
        /// <param name="name">������Դ����</param>
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
            //����һ����Ч
            ResHelper.AsyncLoad<AudioClip>("Audio/Bgm/" + name, clip =>
            {
                //�����bgm���ڲ��� ���Ȱ����������� 1 - 0 �ٲ��ŵ�ǰ���� 0 - 1                
                if (mBGM.isPlaying)
                {
                    mFade.SetState(FadeState.FadeOut, () => Play(clip));
                }
                //���û�ж����ڲ���
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