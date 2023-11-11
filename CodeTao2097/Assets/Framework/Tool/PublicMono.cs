using System;

namespace QFramework
{
    public class PublicMono : MonoSingle<PublicMono>
    {
        public  event Action OnUpdate;
        public  event Action OnFixedUpdate;
        public  event Action OnLateUpdate;
        public  event Action OnGuiCall;

        private void Update()
        {
            OnUpdate?.Invoke();
        }
        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }
        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }
        private void OnGUI()
        {
            OnGuiCall?.Invoke();
        }
    }
}