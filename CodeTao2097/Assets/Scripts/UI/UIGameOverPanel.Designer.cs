using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace CodeTao
{
	// Generate Id:18d80e4e-c211-4d87-8521-97f6a7e26568
	public partial class UIGameOverPanel
	{
		public const string Name = "UIGameOverPanel";
		
		
		private UIGameOverPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIGameOverPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGameOverPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGameOverPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
