using System;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public interface IInputDeviceMgrSystem : ISystem
    {
        void OnEnable();
        void OnDisable();
        void RegisterDeviceChange<T>(Action fun) where T : InputDevice;
    }
    /// <summary>
    /// 输入设备检测器
    /// </summary>
    public class InputDeviceMgrSystem : AbstractSystem, IInputDeviceMgrSystem
    {
        // 绑定输入设备切换事件
        private Dictionary<Type, Action> mDeviceSwitchTable;
        // 当前输入设备
        private InputDevice mCurDevice;
        /// <summary>
        /// 注册设备切换事件
        /// </summary>
        void IInputDeviceMgrSystem.RegisterDeviceChange<T>(Action fun)
        {
            var type = typeof(T);
            if (mDeviceSwitchTable.ContainsKey(type))
            {
                mDeviceSwitchTable[type] += fun;
            }
            else
            {
                mDeviceSwitchTable.Add(type, fun);
            }
        }
        // 初始化系统
        protected override void OnInit()
        {
            mDeviceSwitchTable = new Dictionary<Type, Action>();
        }
        /// <summary>
        /// 激活设备切换
        /// </summary>
        public void OnEnable()
        {
            InputSystem.onActionChange += DetectCurrentInputDevice;
            InputSystem.onDeviceChange += OnDeviceChanged;
        }
        /// <summary>
        /// 关闭设备切换
        /// </summary>
        public void OnDisable()
        {
            InputSystem.onActionChange -= DetectCurrentInputDevice;
            InputSystem.onDeviceChange -= OnDeviceChanged;
            // 清空事件
            mDeviceSwitchTable.Clear();
        }
        // 检测当前输入设备
        private void DetectCurrentInputDevice(object obj, InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed) return;
            SwitchingDevice((obj as InputAction).activeControl.device);
        }
        // 设备变更事件
        private void OnDeviceChanged(InputDevice device, InputDeviceChange change)
        {
            switch (change)
            {
                case InputDeviceChange.Reconnected:
                    Debug.Log(device + "重新链接");
                    // 切换为当前连接设备
                    SwitchingDevice(device);
                    break;
                case InputDeviceChange.Disconnected:
                    Debug.Log(device + "断开链接");
                    // 切换为当前设备以外的设备 暂时先用键盘代替
#if UNITY_STANDALONE || UNITY_EDITOR
                    if (device is Gamepad) device = Keyboard.current;
                    SwitchingDevice(device);
#endif
                    break;
            }
        }
        // 切换设备
        private void SwitchingDevice(InputDevice device)
        {
            // 如果传入的设备为空 且不同于当前输入设备
            if (device == null || device.Equals(mCurDevice)) return;

            mCurDevice = device;

            Type type = null;

            if (device is Keyboard) type = typeof(Keyboard);
            else if (device is Gamepad) type = typeof(Gamepad);
            else if (device is Pointer) type = typeof(Pointer);
            else if (device is Joystick) type = typeof(Joystick);

            // 如果转换成功且当前类型被注册 就执行对应的委托方法 
            if (type == null || !mDeviceSwitchTable.TryGetValue(type, out var callback)) return;

            callback?.Invoke();
        }
    }
}