using System;
using CodeTao;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.SceneManagement;

namespace CodeTao
{
	public class UIGameOverPanelData : UIPanelData
	{
	}
	
	/// <summary>
	/// 游戏失败界面的UI
	/// </summary>
	public partial class UIGameOverPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGameOverPanelData ?? new UIGameOverPanelData();
			
			Time.timeScale = 0;
			ActionKit.OnUpdate.Register(() =>
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					Global.Instance.Reset();
					this.CloseSelf();
					SceneManager.LoadScene("SampleScene");
					Time.timeScale = 1;
				}
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
		}

		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
