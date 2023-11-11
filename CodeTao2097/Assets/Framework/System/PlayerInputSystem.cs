using UnityEngine;
using UnityEngine.InputSystem;

namespace QFramework
{
    public interface IPlayerInputSystem : ISystem
    {
        void Enable();
        void Disable();
    }
    public struct AttackInputEvent { public bool trigger; }
    public struct SwitchUpInputEvent { }
    /// <summary>
    /// 玩家输入系统
    /// </summary>
    public class PlayerInputSystem : AbstractSystem, IPlayerInputSystem, PlayerInputActions.IGamePlayActions
    {
        private PlayerInputActions mControls;
        private MousePosChangeEvent mouseEvent;

        protected override void OnInit()
        {
            mControls = new PlayerInputActions();
            mControls.GamePlay.SetCallbacks(this);
        }
        void IPlayerInputSystem.Enable()
        {
            mControls.GamePlay.Enable();
        }
        void IPlayerInputSystem.Disable()
        {
            mControls.GamePlay.Disable();
        }
        void PlayerInputActions.IGamePlayActions.OnMove(InputAction.CallbackContext context)
        {
            this.SendEvent(new DirInputEvent() { dir = context.ReadValue<Vector2>() });
        }
        void PlayerInputActions.IGamePlayActions.OnInteractive(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            this.SendEvent<InteractiveKeyInputEvent>();
        }
        void PlayerInputActions.IGamePlayActions.OnTapLeft(InputAction.CallbackContext context)
        {
            this.SendEvent(new MouseLeftBtnEvent() { trigger = context.ReadValueAsButton() });
        }
        void PlayerInputActions.IGamePlayActions.OnTapRight(InputAction.CallbackContext context)
        {
            this.SendEvent(new MouseRightBtnEvent() { trigger = context.ReadValueAsButton() });
        }
        void PlayerInputActions.IGamePlayActions.OnTapMiddle(InputAction.CallbackContext context)
        {
            this.SendEvent(new MouseMiddleBtnEvent() { trigger = context.ReadValueAsButton() });
        }
        void PlayerInputActions.IGamePlayActions.OnMousePos(InputAction.CallbackContext context)
        {
            mouseEvent.pos = context.ReadValue<Vector2>();
            this.SendEvent(mouseEvent);
        }
        void PlayerInputActions.IGamePlayActions.OnMouseDelta(InputAction.CallbackContext context)
        {
            this.SendEvent(new MouseDeltaEvent() { delta = context.ReadValue<Vector2>() });
        }
        void PlayerInputActions.IGamePlayActions.OnMouseScroll(InputAction.CallbackContext context)
        {
            this.SendEvent(new MouseScrollEvent() { scroll = context.ReadValue<float>() });
        }
        void PlayerInputActions.IGamePlayActions.OnAttack(InputAction.CallbackContext context)
        {
            this.SendEvent(new AttackInputEvent() { trigger = context.ReadValueAsButton() });
        }

        void PlayerInputActions.IGamePlayActions.OnSwitchUp(InputAction.CallbackContext context)
        {
            if (context.started) this.SendEvent<SwitchUpInputEvent>();
        }
    }
}