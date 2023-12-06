using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace CodeTao
{
	// Generate Id:c3d6c9aa-380d-4e9e-bbad-aec4aec81861
	public partial class UIGamePanel
	{
		public const string Name = "UIGamePanel";
		
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
		[SerializeField]
		public ExpUpgradePanel ExpUpgradePanel;
		
		private UIGamePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			HPBar = null;
			HpValue = null;
			MaxHpValue = null;
			EXPBar = null;
			ExpValue = null;
			MaxExpValue = null;
			LvlValue = null;
			MinuteValue = null;
			SecondValue = null;
			ExpUpgradePanel = null;
			
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
