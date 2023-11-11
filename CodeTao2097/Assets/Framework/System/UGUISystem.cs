using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace QFramework
{
    /// <summary>
    /// UI�㼶
    /// </summary>
    public enum UILayer
    {
        Bot, Mid, Top, System
    }
    public interface IUGUISystem : ISystem
    {
        RectTransform Canvas { get; }
        Vector2 CanvasResolution { get; set; }
        Transform GetLayerFather(UILayer layer);

        T GetPanel<T>() where T : UIPanel;

        void ShowPanel<T>(UILayer layer, Action<T> callBack = null) where T : UIPanel;
        void ShowPanel<T>(string panelName, UILayer layer, Action<T> callBack = null) where T : UIPanel;
        void HidePanel<T>(string panelName, bool isDestroy = false) where T : UIPanel;
        void HidePanel<T>(bool isDestroy = false) where T : UIPanel;
        void ShowOrHidePanel<T>(string panelName, UILayer layer, bool isDestroy = false, Action<T> callBack = null) where T : UIPanel;
        void ShowOrHidePanel<T>(UILayer layer, bool isDestroy = false, Action<T> callBack = null) where T : UIPanel;
        void Clear();
    }
    /// <summary>
    /// UI����ϵͳ
    /// ����������ʾ����� �ṩ��ʾ�����ص���Ϣ
    /// </summary>
    public class UGUISystem : AbstractSystem, IUGUISystem
    {
        // �ѿ������
        private Dictionary<string, UIPanel> mOpenPanels;
        /// <summary>
        /// ��¼UI�Ļ���
        /// </summary>
        private RectTransform mCanvas;
        RectTransform IUGUISystem.Canvas => mCanvas;
        /// <summary>
        /// ����������׼
        /// </summary>
        private CanvasScaler mScaler;
        /// <summary>
        /// �����ֱ���
        /// </summary>
        Vector2 IUGUISystem.CanvasResolution
        {
            get => mScaler.referenceResolution;
            set => mScaler.referenceResolution = value;
        }
        /// <summary>
        /// UI�㼶����
        /// </summary>
        private Transform mBot, mMid, mTop, mSystem;
        /// <summary>
        /// ��ʼ��ϵͳ
        /// </summary>
        protected override void OnInit()
        {
            mOpenPanels = new Dictionary<string, UIPanel>();

            var obj = new GameObject("Canvas", typeof(Canvas), typeof(GraphicRaycaster));

            obj.layer = LayerMask.NameToLayer("UI");
            mCanvas = obj.transform as RectTransform;

            obj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            mScaler = obj.AddComponent<CanvasScaler>();
            mScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            mScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            mScaler.referenceResolution = new Vector2(1920, 1080);

            mBot = new GameObject("Bot").transform;
            mBot.SetParent(mCanvas);
            mBot.localPosition = Vector2.zero;

            mMid = new GameObject("Mid").transform;
            mMid.SetParent(mCanvas);
            mMid.localPosition = Vector2.zero;

            mTop = new GameObject("Top").transform;
            mTop.SetParent(mCanvas);
            mTop.localPosition = Vector2.zero;

            mSystem = new GameObject("System").transform;
            mSystem.SetParent(mCanvas);
            mSystem.localPosition = Vector2.zero;

            GameObject.DontDestroyOnLoad(obj);

            obj = new GameObject("EventSystem", typeof(EventSystem));
            //obj.AddComponent<StandaloneInputModule>();
            obj.AddComponent<InputSystemUIInputModule>();

            GameObject.DontDestroyOnLoad(obj);
        }
        void IUGUISystem.ShowOrHidePanel<T>(UILayer layer, bool isDestroy, Action<T> callBack) => ShowOrHidePanel<T>(typeof(T).Name, layer, isDestroy, callBack);
        public void ShowOrHidePanel<T>(string panelName, UILayer layer, bool isDestroy, Action<T> callBack) where T : UIPanel
        {
            // ���������
            if (mOpenPanels.TryGetValue(panelName, out var panel))
            {
                // �����忪�� �͹ر�
                if (panel.IsOpen)
                {
                    panel.HideMe(); // ִ�����Ĺر��߼�
                    callBack?.Invoke(panel as T);
                    if (isDestroy) //�����Ҫ���� 
                    {
                        mOpenPanels.Remove(panelName);
                        GameObject.Destroy(panel.gameObject);
                    }
                    else
                    {
                        panel.gameObject.SetActive(false);
                    }
                }
                else
                {
                    // ����رվ� ��                    
                    panel.gameObject.SetActive(true);
                    panel.ShowMe();
                    callBack?.Invoke(panel as T);
                }
            }
            // �����岻����
            else
            {
                LoadPanel<T>(panelName, layer, callBack);
            }
        }
        private void LoadPanel<T>(string panelName, UILayer layer, Action<T> callBack) where T : UIPanel
        {
            //���������û�б����ع� ��ʵ����
            ResHelper.AsyncLoad<GameObject>("Panel/" + panelName, o =>
            {
                o.transform.SetParent(GetLayerFather(layer));

                o.transform.localPosition = Vector3.zero;
                o.transform.localScale = Vector3.one;

                (o.transform as RectTransform).offsetMax = Vector2.zero;
                (o.transform as RectTransform).offsetMin = Vector2.zero;

                T panel = o.GetComponent<T>();

                panel.ShowMe();
                //���������
                callBack?.Invoke(panel);
                //��ӵ�UI�б�
                mOpenPanels.Add(panelName, panel);
            });
        }
        /// <summary>
        /// ��ʾ���
        /// </summary>
        /// <typeparam name="T">���ű�����</typeparam>
        /// <param name="layer">������ڲ㼶</param>
        /// <param name="callBack">��崴����ɸ���������</param>
        void IUGUISystem.ShowPanel<T>(UILayer layer, Action<T> callBack)
        {
            ShowPanel<T>(typeof(T).Name, layer, callBack);
        }
        public void ShowPanel<T>(string panelName, UILayer layer, Action<T> callBack) where T : UIPanel
        {
            //����������Ѿ����ع� ��ֱ����ʾ���
            if (mOpenPanels.TryGetValue(panelName, out UIPanel basePanel))
            {
                //�����屻���� ֱ������
                if (basePanel.IsOpen) return;
                basePanel.gameObject.SetActive(true);
                basePanel.ShowMe();
                callBack?.Invoke(basePanel as T);
                return;
            }
            LoadPanel<T>(panelName, layer, callBack);
        }
        /// <summary>
        /// �������
        /// </summary>
        /// <param name="isDestroy">�Ƿ��������</param>
        void IUGUISystem.HidePanel<T>(bool isDestroy)
        {
            HidePanel<T>(typeof(T).Name, isDestroy);
        }
        /// <summary>
        /// ��ȡһ���ѿ��������ű�
        /// </summary>
        T IUGUISystem.GetPanel<T>() => mOpenPanels.TryGetValue(typeof(T).Name, out UIPanel panel) ? panel as T : null;
        /// <summary>
        /// ��ȡ Canvas �µĲ㼶
        /// </summary>
        /// <param name="layer">UI�㼶</param>
        public Transform GetLayerFather(UILayer layer)
        {
            switch (layer)
            {
                case UILayer.Bot: return mBot;
                case UILayer.Mid: return mMid;
                case UILayer.Top: return mTop;
                case UILayer.System: return mSystem;
                default: return null;
            }
        }
        void IUGUISystem.Clear()
        {
            if (mOpenPanels.Count == 0) return;
            foreach (UIPanel panel in mOpenPanels.Values)
                GameObject.Destroy(panel.gameObject);
            mOpenPanels.Clear();
        }

        public void HidePanel<T>(string panelName, bool isDestroy) where T : UIPanel
        {
            //�������б�������������
            if (!mOpenPanels.TryGetValue(panelName, out UIPanel panel)) return;
            // ִ�����Ĺر��߼�
            panel.HideMe();
            //�����Ҫ���� 
            if (isDestroy)
            {
                GameObject.Destroy(panel.gameObject);
                mOpenPanels.Remove(panelName);
                return;
            }
            //�������Ҫ���� ����崦�ڼ���״̬ 
            if (!panel.IsOpen) return;
            //�����ʧ��
            panel.gameObject.SetActive(false);
        }
    }
}