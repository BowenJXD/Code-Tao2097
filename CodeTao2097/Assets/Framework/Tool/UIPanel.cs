using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace QFramework
{
    /// <summary>
    /// UI���� ��װ��������� �Լ� ע��ί�м�ʹ��[����ʵ�� IController �ӿ�]
    /// �ṩ��ʾ�����ص���Ϊ
    /// </summary>
    public abstract class UIPanel : MonoBehaviour
    {
        /// <summary>
        /// �������пؼ�
        /// </summary>
        private Dictionary<string, List<UIBehaviour>> mControlDic;
        /// <summary>
        /// �ж�����Ƿ񼤻�
        /// </summary>
        protected bool mIsOpen;
        public bool IsOpen => mIsOpen;
        protected virtual void OnEnable() => mIsOpen = true;
        protected virtual void OnDisable() => mIsOpen = false;
        /// <summary>
        /// ��ʾ�Լ� ��ʾ���ʱִ�е��߼�
        /// </summary>
        public virtual void ShowMe() { }
        /// <summary>
        /// �����Լ�
        /// </summary>
        public virtual void HideMe() { }
        /// <summary>
        /// ����ʵ�ֵ���¼�
        /// </summary>
        /// <param name="btnName">��ť����</param>
        protected virtual void OnClick(string btnName) { }
        /// <summary>
        /// ѡ���¼�
        /// </summary>
        /// <param name="toggleName">��ѡ��ť����</param>
        /// <param name="value">�Ƿ�ѡ��</param>
        protected virtual void OnValueChanged(string toggleName, bool value) { }
        /// <summary>
        /// ��ȡ����UI���
        /// </summary>
        protected virtual void Awake()
        {
            mControlDic = new Dictionary<string, List<UIBehaviour>>();

            FindChildrenControl<Button>((name, control) => control.onClick.AddListener(() => OnClick(name)));
            FindChildrenControl<Toggle>((name, control) => control.onValueChanged.AddListener(isSelect => OnValueChanged(name, isSelect)));
        }
        /// <summary>
        /// �õ���Ӧ���ֵĶ�Ӧ�ؼ��ű�
        /// </summary>
        /// <param name="controlName">�ؼ���</param>
        /// <returns>��Ӧ�ؼ�</returns>
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
        /// �ҵ���常�ڵ������ж�Ӧ�ؼ�
        /// </summary>
        /// <typeparam name="T">�ؼ�����</typeparam>
        protected void FindChildrenControl<T>(Action<string, T> callback = null) where T : UIBehaviour
        {
            //�õ������ӿؼ�
            T[] controls = this.GetComponentsInChildren<T>();
            //����ҵ� ���������ӿؼ�
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