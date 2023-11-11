using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class GameMgrSystem : AbstractSystem
    {
        protected override void OnInit()
        {
            this.GetSystem<IInputDeviceMgrSystem>().OnEnable();
            this.GetSystem<IPlayerInputSystem>().Enable();
        }
    }
}