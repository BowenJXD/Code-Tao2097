using UnityEngine;

namespace QFramework
{
    public interface IEvent { }
    public struct InteractiveKeyInputEvent { }    
    public struct DirInputEvent { public Vector2 dir; }
    public struct MousePosChangeEvent { public Vector2 pos; }
    public struct MouseDeltaEvent { public Vector2 delta; }
    public struct MouseScrollEvent { public float scroll; }
    public struct MouseLeftBtnEvent { public bool trigger; }
    public struct MouseRightBtnEvent { public bool trigger; }
    public struct MouseMiddleBtnEvent { public bool trigger; }
}