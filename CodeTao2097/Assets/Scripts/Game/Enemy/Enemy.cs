using System;
using GraphProcessor;
using UnityEngine;
using QFramework;

namespace CodeTao
{
	public partial class Enemy : MoveController
	{
		private void Start()
		{
			SelfNavAgent.updateRotation = false;
			SelfNavAgent.updateUpAxis = false;

			Defencer.TakeDamageAfter += (damage) =>
			{
				Sprite.color = Color.red;
				ActionKit.Delay(Defencer.DMGCD, () =>
				{
					if (!this) return;
					Sprite.color = Color.white;
				}).Start(this);
			};

			HitBox.OnTriggerEnter2DEvent((col) =>
			{
				if (Util.IsTagIncluded(Util.GetTagFromParent(col), Damager.damagingTags))
				{
					Defencer target = Util.GetComponentInSiblings<Defencer>(col);
					DamageManager.Instance.ExecuteDamage(Damager, target, Attacker);
				}
			}).UnRegisterWhenGameObjectDestroyed(this);
		}

		private void Update()
		{
			if (Player.Instance)
			{
				SelfNavAgent.speed = SPD;
				SelfNavAgent.SetDestination(Player.Instance.transform.position);
			}
		}
	}
}
