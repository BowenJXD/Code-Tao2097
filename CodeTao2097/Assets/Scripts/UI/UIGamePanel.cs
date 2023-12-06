using System;
using CodeTao;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace CodeTao
{
	public class UIGamePanelData : UIPanelData
	{
	}
	public partial class UIGamePanel : UIPanel
	{
		public ExpController expController;
		public Defencer defencer;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGamePanelData ?? new UIGamePanelData();

			if (!expController)
			{
				expController = Player.Instance.ExpController;
			}
			
			if (!defencer)
			{
				defencer = Player.Instance.Defencer;
			}
			
			// HP
			defencer.HP.RegisterWithInitValue(hp =>
			{
				HpValue.text = hp.ToString();
				HPBar.value = hp / defencer.MaxHP.Value;
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			defencer.MaxHP.RegisterWithInitValue(maxHp =>
			{
				MaxHpValue.text = maxHp.ToString();
				HPBar.value = defencer.HP.Value / maxHp;
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			// Experience
			expController.EXP.RegisterWithInitValue(exp =>
			{
				ExpValue.text = exp.ToString();
				EXPBar.value = exp / expController.RequiredEXP();
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			// Level
			expController.LVL.RegisterWithInitValue(lvl =>
			{
				LvlValue.text = lvl.ToString();
				MaxExpValue.text = expController.RequiredEXP().ToString();
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			expController.LVL.Register(lvl =>
			{
				Time.timeScale = 0;
				ExpUpgradePanel.Show();
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			// Upgrade
			ExpUpgradePanel.Hide();
			
			// Timer
			Global.GameTime.RegisterWithInitValue(time =>
			{
				SetTimer(time);
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			ActionKit.OnUpdate.Register(() =>
			{
				Global.GameTime.Value += Time.deltaTime * Time.timeScale;
				if (Global.IsPass)
				{
					UIKit.OpenPanel<UIGamePassPanel>();
				}
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
		}

		public void SetTimer(float totalSeconds)
		{
			if (Time.frameCount % 30 == 0)
			{
				var currentSecondsInt = Mathf.FloorToInt(totalSeconds);
				var seconds = currentSecondsInt % 60;
				var minutes = currentSecondsInt / 60;
				MinuteValue.text = $"{minutes}";
				SecondValue.text = $"{seconds:00}";
			}
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
