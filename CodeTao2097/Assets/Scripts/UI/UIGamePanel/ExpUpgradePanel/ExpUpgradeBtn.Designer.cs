/****************************************************************************
 * 2023.12 AARON
 ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace CodeTao
{
	public partial class ExpUpgradeBtn
	{
		[SerializeField] public UnityEngine.UI.Button UpgradeBtn;
		[SerializeField] public TMPro.TextMeshProUGUI BtnText;

		public void Clear()
		{
			UpgradeBtn = null;
			BtnText = null;
		}

		public override string ComponentName
		{
			get { return "ExpUpgradeBtn";}
		}
	}
}
