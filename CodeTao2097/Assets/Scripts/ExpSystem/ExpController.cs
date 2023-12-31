﻿using System;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class ExpController : ViewController
    {
        public BindableProperty<float> EXP = new BindableProperty<float>(0);
        
        public BindableProperty<int> LVL = new BindableProperty<int>(1);
        
        public BindableStat EXPRate = new BindableStat(1);

        private void Start()
        {
        }
        
        public Action levelUpAfter;
        
        public void LevelUp()
        {
            LVL.Value += 1;
            levelUpAfter?.Invoke();
        }
        
        public float AlterEXP(float exp)
        {
            float result = exp * EXPRate + EXP.Value;
            while (result >= RequiredEXP())
            {
                result -= RequiredEXP(); 
                LevelUp();
            }
            EXP.Value = result;
            return result;
        }
        
        public float RequiredEXP()
        {
            return LVL.Value + 1;
        }
        
        public void Reset()
        {
            EXP = new BindableProperty<float>(0);
            LVL = new BindableProperty<int>(1);
            EXPRate.Reset();
        }
        
        private void OnDisable()
        {
            Reset();
        }
    }
}