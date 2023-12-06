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
        
        public static bool IsPass = false;
        public static float GameDuration = 60;
        public static BindableProperty<float> GameTime = new BindableProperty<float>(0);
        
        public UnityEvent OnGameStart = new UnityEvent();

        private void Start()
        {
            Random = new Random(RandomSeed);
            OnGameStart.Invoke();
        }

        public void Reset()
        {
            IsPass = false;
            GameTime = new BindableProperty<float>(0);
        }
    }
}