/****************************************************************************
 * 2023.12 AARON
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.Pool;

namespace CodeTao
{
	public partial class ExpUpgradePanel : UIElement
	{
		ItemManager itemManager;
		List<ExpUpgradeBtn> btns = new List<ExpUpgradeBtn>();
		ObjectPool<ExpUpgradeBtn> ExpUpBtnPool;

		private void Awake()
		{
			if (!itemManager) itemManager = ItemManager.Instance;
			
			ExpUpBtnPool = new ObjectPool<ExpUpgradeBtn>(
				() =>
				{
					var btn = ExpUpBtnTemplate.InstantiateWithParent(UpgradeRoot);
					btn.gameObject.SetActive(true);
					return btn;
				},
				btn =>
				{
					btn.Show();
				},
				(btn) =>
				{
					btn.Hide();
				},
				(btn) =>
				{
					btn.Clear();
				},
				true, 4
			);
		}

		protected override void OnShow()
		{
			base.OnShow();
			FillBtns();
		}

		void FillBtns()
		{
			var randomItems = itemManager.GetRandomItems(3);
			if (randomItems == null) return;
			
			foreach (var btn in btns)
			{
				ExpUpBtnPool.Release(btn);
			}
			btns.Clear();
			
			foreach (var item in randomItems)
			{
				var btn = ExpUpBtnPool.Get();
				btn.BtnText.text = item.GetDescription();
				btn.UpgradeBtn.onClick.AddListener(() =>
				{
					Time.timeScale = 1.0f;
					item.Upgrade();
					this.Hide();
				});
				btns.Add(btn);
			}
		}

		protected override void OnBeforeDestroy()
		{
		}

	}
}