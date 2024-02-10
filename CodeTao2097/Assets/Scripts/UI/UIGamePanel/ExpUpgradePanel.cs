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
	/// <summary>
	/// 升级面板的UI，包括各种升级选项
	/// </summary>
	public partial class ExpUpgradePanel : UIElement
	{
		ItemManager itemManager;
		List<ExpUpgradeBtn> btns = new List<ExpUpgradeBtn>();
		ObjectPool<ExpUpgradeBtn> ExpUpBtnPool;
		private Inventory _playerInventory;

		private void Awake()
		{
			if (!itemManager) itemManager = ItemManager.Instance;
			_playerInventory = Player.Instance.GetComp<Inventory>();
			
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
					btn.UpgradeBtn?.onClick.RemoveAllListeners();
				},
				(btn) =>
				{
					btn.Clear();
				},
				true, 4
			);
		}
		
		public void Show(ItemType[] itemTypes)
		{
			base.Show();
			Time.timeScale = 0;
			FillBtns(itemTypes);
		}

		void FillBtns(ItemType[] itemTypes)
		{
			var randomItems = itemManager.GetRandomUpgradeItems(3, itemTypes);
			if (randomItems == null) return;
			
			foreach (var btn in btns)
			{
				ExpUpBtnPool.Release(btn);
			}
			btns.Clear();
			
			foreach (var item in randomItems)
			{
				var btn = ExpUpBtnPool.Get();
				btn.BtnText.text = item.GetUpgradeDescription();
				btn.UpgradeBtn.onClick.AddListener(() =>
				{
					if (item.LVL == 0)
					{
						item.AddToContainer(_playerInventory);
					}
					else
					{
						item.AlterLVL();
					}
					Time.timeScale = 1.0f;
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