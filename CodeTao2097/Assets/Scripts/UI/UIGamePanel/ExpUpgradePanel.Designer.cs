/****************************************************************************
 * 2023.12 AARON
 ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace CodeTao
{
	public partial class ExpUpgradePanel
	{
		[SerializeField] public RectTransform UpgradeRoot;
		[SerializeField] public ExpUpgradeBtn ExpUpBtnTemplate;

		public void Clear()
		{
			UpgradeRoot = null;
			ExpUpBtnTemplate = null;
		}

		public override string ComponentName
		{
			get { return "ExpUpgradePanel";}
		}
	}
}
