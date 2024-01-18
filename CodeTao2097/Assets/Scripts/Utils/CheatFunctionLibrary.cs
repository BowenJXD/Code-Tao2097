using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CodeTao
{
    /// <summary>
    /// 作弊功能库，用于测试
    /// </summary>
    public class CheatFunctionLibrary : MonoSingleton<CheatFunctionLibrary>
    {
        public List<UnityEvent> activateEvents;
        PlayerInput playerInput;
        ExpController expController;
        Defencer defencer;
        Attacker attacker;
        LoopTask expUpTask;
        EnemyGenerator enemyGenerator;
        bool f4Toggle = false;
        public float expUpInterval = 10f;

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            playerInput.actions["1"].performed += ctx => F1();
            playerInput.actions["2"].performed += ctx => F2();
            playerInput.actions["3"].performed += ctx => F3();
            playerInput.actions["4"].performed += ctx => F4();
            
            expController = Player.Instance.GetComp<ExpController>();
            defencer = Player.Instance.GetComp<Defencer>();
            attacker = Player.Instance.GetComp<Attacker>();
            enemyGenerator = FindObjectOfType<EnemyGenerator>();
            
            foreach (UnityEvent activateEvent in activateEvents)
            {
                activateEvent.Invoke();
            }
        }

        public void F1()
        {
            if (expController.EXPRate.Value > 0f)
            {
                expController.EXPRate.Value = 0f;
            }
            else
            {
                expController.LevelUp();
            }
        }

        public void F2()
        {
            defencer.IsInCD = true;
        }

        public void F3()
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemies)
            {
                enemy.GetComp<Defencer>().IsInCD = true;
            }
        }

        public void F4()
        {
            if (f4Toggle)
            {
                enemyGenerator.Pause();
            }
            else
            {
                enemyGenerator.Resume();
            }
            f4Toggle = !f4Toggle;
        }
    }
}