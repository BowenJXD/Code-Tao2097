using UnityEngine;
using QFramework;

namespace CodeTao
{
	/// <summary>
	/// 获取附近的collectable的组件
	/// </summary>
	public partial class Collector : UnitComponent
	{
		public BindableStat range = new BindableStat(3);
		public Collider2D col;
		
		void Start()
		{
			col = this.GetComponentInDescendants<Collider2D>();
			
			range.RegisterWithInitValue(value =>
			{
				col.LocalScale(value);
			}).UnRegisterWhenGameObjectDestroyed(this);
			
			col.OnTriggerEnter2DEvent(triggerCol =>
			{
				Collectable collectable = triggerCol.GetUnit() as Collectable;
				if (collectable)
				{
					if (collectable.ValidateCollision(col))
					{
						collectable.StartCollection(transform);
					}
				}
			}).UnRegisterWhenGameObjectDestroyed(this);
		}
		
		private void OnDisable()
		{
			range.Reset();
		}
	}
}
