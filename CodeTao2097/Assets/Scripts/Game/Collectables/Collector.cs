using UnityEngine;
using QFramework;

namespace CodeTao
{
	public partial class Collector : ViewController
	{
		public BindableStat range = new BindableStat(3);
		
		void Start()
		{
			range.RegisterWithInitValue(value =>
			{
				CollectRange.radius = value;
			}).UnRegisterWhenGameObjectDestroyed(this);
			
			CollectRange.OnTriggerEnter2DEvent(col =>
			{
				Collectable collectable = ComponentUtil.GetComponentFromUnit<Collectable>(col);
				if (collectable)
				{
					if (collectable.ValidateCollision(CollectRange))
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
