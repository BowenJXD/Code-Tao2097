using System;
using QFramework;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;


namespace CodeTao
{
    /// <summary>
    /// 全局控制器，用于控制游戏的全局变量
    /// </summary>
    public class Global : MonoSingleton<Global>
    {
        public int RandomSeed = 0;

        public float duration = 60;
        public Random Random;

        public float DefenceFactor = 10;
        public static BindableProperty<bool> IsPass = new BindableProperty<bool>(false);
        public static BindableProperty<float> GameDuration;
        public static BindableProperty<float> GameTime = new BindableProperty<float>(0);
        
        public UnityEvent OnGameStart = new UnityEvent();

        private void Awake()
        {
            Random = new Random(RandomSeed);
            OnGameStart.Invoke();
            
            GameDuration = new BindableProperty<float>(duration);
            
            // pass the game when game time reaches the duration
            GameTime.RegisterWithInitValue(value =>
            {
                if (value >= GameDuration)
                {
                    IsPass.Value = true;
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            // open pass panel when game is passed
            IsPass.RegisterWithInitValue(value =>
            {
                if (value)
                {
                    UIKit.OpenPanel<UIGamePassPanel>();
                }
            }).UnRegisterWhenGameObjectDestroyed(this);

            UnitController[] unitControllers = FindObjectsOfType<UnitController>(true);
            foreach (var unitController in unitControllers)
            {
                unitController.OnSceneLoaded();
            }
        }

        private void Start()
        {
            // open game over panel when player is destroyed
            Player.Instance.onDeinit += () =>
            {
                IsPass.Value = false;
                UIKit.OpenPanel<UIGameOverPanel>();
            };
        }
        
        public void Reset()
        {
            IsPass.Value = new BindableProperty<bool>(false);
            GameTime = new BindableProperty<float>(0);
        }
    }
}