using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace QFramework
{
    /// <summary>
    /// UI基类 封装找组件功能 以及 注册委托简化使用[子类实现 IController 接口]
    /// 提供显示或隐藏的行为
    /// </summary>
    public abstract class UIPanel : MonoBehaviour
    {
        /// <summary>
        /// 保存所有控件
        /// </summary>
        private Dictionary<string, List<UIBehaviour>> mControlDic;
        /// <summary>
        /// 判断面板是否激活
        /// </summary>
        protected bool mIsOpen;
        public bool IsOpen => mIsOpen;
        protected virtual void OnEnable() => mIsOpen = true;
        protected virtual void OnDisable() => mIsOpen = false;
        /// <summary>
        /// 显示自己 显示面板时执行的逻辑
        /// </summary>
        public virtual void ShowMe() { }
        /// <summary>
        /// 隐藏自己
        /// </summary>
        public virtual void HideMe() { }
        /// <summary>
        /// 子类实现点击事件
        /// </summary>
        /// <param name="btnName">按钮名字</param>
        protected virtual void OnClick(string btnName) { }
        /// <summary>
        /// 选择事件
        /// </summary>
        /// <param name="toggleName">单选按钮名字</param>
        /// <param name="value">是否选中</param>
        protected virtual void OnValueChanged(string toggleName, bool value) { }
        /// <summary>
        /// 获取所有UI组件
        /// </summary>
        protected virtual void Awake()
        {
            mControlDic = new Dictionary<string, List<UIBehaviour>>();

            FindChildrenControl<Button>((name, control) => control.onClick.AddListener(() => OnClick(name)));
            FindChildrenControl<Toggle>((name, control) => control.onValueChanged.AddListener(isSelect => OnValueChanged(name, isSelect)));
        }
        /// <summary>
        /// 得到对应名字的对应控件脚本
        /// </summary>
        /// <param name="controlName">控件名</param>
        /// <returns>对应控件</returns>
        protected T GetControl<T>(string controlName) where T : UIBehaviour
        {
            if (mControlDic.TryGetValue(controlName, out var controls))
            {
                for (int i = controls.Count - 1; i >= 0; i--)
                    if (controls[i] is T) return controls[i] as T;
            }
            return null;
        }
        /// <summary>
        /// 找到面板父节点下所有对应控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        protected void FindChildrenControl<T>(Action<string, T> callback = null) where T : UIBehaviour
        {
            //得到所有子控件
            T[] controls = this.GetComponentsInChildren<T>();
            //如果找到 遍历所有子控件
            for (int i = controls.Length - 1; i >= 0; i--)
            {
                T control = controls[i];

                string objName = control.gameObject.name;

                callback?.Invoke(objName, control);

                if (mControlDic.ContainsKey(objName)) mControlDic[objName].Add(control);
                else mControlDic.Add(objName, new List<UIBehaviour>() { control });
            }
        }
    }
}