using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace CodeTao
{
	// Generate Id:e30e05ce-7161-4808-9466-5121eca1211a
	public partial class UIGamePanel
	{
		public const string Name = "UIGamePanel";
		
		[SerializeField]
		public RectTransform Upgrades;
		[SerializeField]
		public UnityEngine.UI.Button BtnDMGUp;
		[SerializeField]
		public UnityEngine.UI.Button BtnFrequencyUp;
		[SerializeField]
		public UnityEngine.UI.Slider HPBar;
		[SerializeField]
		public TMPro.TextMeshProUGUI HpValue;
		[SerializeField]
		public TMPro.TextMeshProUGUI MaxHpValue;
		[SerializeField]
		public UnityEngine.UI.Slider EXPBar;
		[SerializeField]
		public TMPro.TextMeshProUGUI ExpValue;
		[SerializeField]
		public TMPro.TextMeshProUGUI MaxExpValue;
		[SerializeField]
		public TMPro.TextMeshProUGUI LvlValue;
		[SerializeField]
		public TMPro.TextMeshProUGUI MinuteValue;
		[SerializeField]
		public TMPro.TextMeshProUGUI SecondValue;
		
		private UIGamePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Upgrades = null;
			BtnDMGUp = null;
			BtnFrequencyUp = null;
			HPBar = null;
			HpValue = null;
			MaxHpValue = null;
			EXPBar = null;
			ExpValue = null;
			MaxExpValue = null;
			LvlValue = null;
			MinuteValue = null;
			SecondValue = null;
			
			mData = null;
		}
		
		public UIGamePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGamePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGamePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
