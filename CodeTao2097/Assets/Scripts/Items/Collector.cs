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
		}
	}
}
