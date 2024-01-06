using System;
using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CodeTao
{
    public class CheatFunctionLibrary : MonoSingleton<CheatFunctionLibrary>
    {
        PlayerInput playerInput;
        ExpController expController;
        Defencer defencer;
        Attacker attacker;
        LoopTask expUpTask;
        public float expUpInterval = 10f;

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            playerInput.actions["1"].performed += ctx => F1();
            playerInput.actions["2"].performed += ctx => F2();
            playerInput.actions["3"].performed += ctx => F3();
            
            expController = Player.Instance.ExpController;
            defencer = Player.Instance.Defencer;
            attacker = Player.Instance.Attacker;
        }

        void F1()
        {
            expUpTask = new LoopTask(this, expUpInterval, () =>
            {
                expController.LevelUp();
            });
            expUpTask.Start();
        }

        void F2()
        {
            defencer.IsInCD = true;
        }

        void F3()
        {
            attacker.ATK.AddModifier(1, EModifierType.Multiplicative, name, ERepetitionBehavior.AddStack);
        }
    }
}