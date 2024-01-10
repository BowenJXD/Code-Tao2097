using QFramework;

namespace CodeTao
{
    /// <summary>
    /// 游戏UI管理器
    /// </summary>
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