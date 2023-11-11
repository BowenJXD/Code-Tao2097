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
    /// UI层级
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
    /// UI管理系统
    /// 管理所有显示的面板 提供显示和隐藏等信息
    /// </summary>
    public class UGUISystem : AbstractSystem, IUGUISystem
    {
        // 已开启面板
        private Dictionary<string, UIPanel> mOpenPanels;
        /// <summary>
        /// 记录UI的画布
        /// </summary>
        private RectTransform mCanvas;
        RectTransform IUGUISystem.Canvas => mCanvas;
        /// <summary>
        /// 画布比例标准
        /// </summary>
        private CanvasScaler mScaler;
        /// <summary>
        /// 画布分辨率
        /// </summary>
        Vector2 IUGUISystem.CanvasResolution
        {
            get => mScaler.referenceResolution;
            set => mScaler.referenceResolution = value;
        }
        /// <summary>
        /// UI层级对象
        /// </summary>
        private Transform mBot, mMid, mTop, mSystem;
        /// <summary>
        /// 初始化系统
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
            // 如果面板存在
            if (mOpenPanels.TryGetValue(panelName, out var panel))
            {
                // 如果面板开着 就关闭
                if (panel.IsOpen)
                {
                    panel.HideMe(); // 执行面板的关闭逻辑
                    callBack?.Invoke(panel as T);
                    if (isDestroy) //如果需要销毁 
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
                    // 如果关闭就 打开                    
                    panel.gameObject.SetActive(true);
                    panel.ShowMe();
                    callBack?.Invoke(panel as T);
                }
            }
            // 如果面板不存在
            else
            {
                LoadPanel<T>(panelName, layer, callBack);
            }
        }
        private void LoadPanel<T>(string panelName, UILayer layer, Action<T> callBack) where T : UIPanel
        {
            //如果这个面板没有被加载过 就实例化
            ResHelper.AsyncLoad<GameObject>("Panel/" + panelName, o =>
            {
                o.transform.SetParent(GetLayerFather(layer));

                o.transform.localPosition = Vector3.zero;
                o.transform.localScale = Vector3.one;

                (o.transform as RectTransform).offsetMax = Vector2.zero;
                (o.transform as RectTransform).offsetMin = Vector2.zero;

                T panel = o.GetComponent<T>();

                panel.ShowMe();
                //当创建完成
                callBack?.Invoke(panel);
                //添加到UI列表
                mOpenPanels.Add(panelName, panel);
            });
        }
        /// <summary>
        /// 显示面板
        /// </summary>
        /// <typeparam name="T">面板脚本类型</typeparam>
        /// <param name="layer">面板所在层级</param>
        /// <param name="callBack">面板创建完成该做的事情</param>
        void IUGUISystem.ShowPanel<T>(UILayer layer, Action<T> callBack)
        {
            ShowPanel<T>(typeof(T).Name, layer, callBack);
        }
        public void ShowPanel<T>(string panelName, UILayer layer, Action<T> callBack) where T : UIPanel
        {
            //如果这个面板已经加载过 就直接显示面板
            if (mOpenPanels.TryGetValue(panelName, out UIPanel basePanel))
            {
                //这个面板被开启 直接跳过
                if (basePanel.IsOpen) return;
                basePanel.gameObject.SetActive(true);
                basePanel.ShowMe();
                callBack?.Invoke(basePanel as T);
                return;
            }
            LoadPanel<T>(panelName, layer, callBack);
        }
        /// <summary>
        /// 隐藏面板
        /// </summary>
        /// <param name="isDestroy">是否销毁面板</param>
        void IUGUISystem.HidePanel<T>(bool isDestroy)
        {
            HidePanel<T>(typeof(T).Name, isDestroy);
        }
        /// <summary>
        /// 获取一个已开启的面板脚本
        /// </summary>
        T IUGUISystem.GetPanel<T>() => mOpenPanels.TryGetValue(typeof(T).Name, out UIPanel panel) ? panel as T : null;
        /// <summary>
        /// 获取 Canvas 下的层级
        /// </summary>
        /// <param name="layer">UI层级</param>
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
            //如果这个列表里面有这个面板
            if (!mOpenPanels.TryGetValue(panelName, out UIPanel panel)) return;
            // 执行面板的关闭逻辑
            panel.HideMe();
            //如果需要销毁 
            if (isDestroy)
            {
                GameObject.Destroy(panel.gameObject);
                mOpenPanels.Remove(panelName);
                return;
            }
            //如果不需要销毁 当面板处于激活状态 
            if (!panel.IsOpen) return;
            //给面板失活
            panel.gameObject.SetActive(false);
        }
    }
}