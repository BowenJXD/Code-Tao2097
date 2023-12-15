using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeTao;
using JetBrains.Annotations;
using QFramework;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;


namespace CodeTao
{
    public class Global : MonoSingleton<Global>
    {
        public int RandomSeed = 0;

        public Random Random;

        public float DefenceFactor = 10;
        public static BindableProperty<bool> IsPass = new BindableProperty<bool>(false);
        public static BindableProperty<float> GameDuration = new BindableProperty<float>(60);
        public static BindableProperty<float> GameTime = new BindableProperty<float>(0);
        
        public UnityEvent OnGameStart = new UnityEvent();

        private void Start()
        {
            Random = new Random(RandomSeed);
            OnGameStart.Invoke();
            
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
            
            // open game over panel when player is destroyed
            Player.Instance.onDestroy += () =>
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