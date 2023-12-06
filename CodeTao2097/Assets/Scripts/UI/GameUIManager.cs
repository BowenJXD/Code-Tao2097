using QFramework;

namespace CodeTao
{
    public partial class GameUIManager : ViewController
    {
        void Start()
        {
            UIKit.OpenPanel<UIGamePanel>();
        }

        private void OnDisable()
        {
            UIKit.ClosePanel<UIGamePanel>();
        }

        private void OnDestroy()
        {
            UIKit.ClosePanel<UIGamePanel>();
        }
    }
}