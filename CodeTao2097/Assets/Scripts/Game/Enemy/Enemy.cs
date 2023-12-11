using System;
using GraphProcessor;
using UnityEngine;
using QFramework;
using UnityEngine.Serialization;

namespace CodeTao
{
	public partial class Enemy : UnitController
	{
		public Transform target;
		public BindableProperty<float> EXPValue = new BindableProperty<float>(1);

		private void Start()
		{
			if (!target)
			{
				target = Player.Instance.transform;
			}

			SelfNavAgent.updateRotation = false;
			SelfNavAgent.updateUpAxis = false;
			
			// Receive element when taking damage
			Defencer.OnTakeDamageFuncs.Add((damage) =>
			{
				ElementOwner?.AddElement(damage.DamageElement);
				return damage;
			});
			
			// Change color after taking damage
			Defencer.TakeDamageAfter += (damage) =>
			{
				Sprite.color = damage.DamageElement.GetColor();
				if (damage.Source)
				{
					Vector2 knockBackDirection = (transform.position - damage.Source.transform.position).normalized;
					SelfRigidbody2D.AddForce(damage.Knockback * knockBackDirection,
						ForceMode2D.Impulse);
				}

				ActionKit.Delay(Defencer.DMGCD, () =>
				{
					if (!this) return;
					Sprite.color = Color.white;
				}).Start(this);
			};

			// Attack player when player is in range
			HitBox.OnTriggerStay2DEvent((col) =>
			{
				UnitController unitController = ComponentUtil.GetComponentInAncestors<UnitController>(col);
				Defencer defencer = ComponentUtil.GetComponentInAncestors<Defencer>(col, 1);
				if (unitController)
				{
					if (Util.IsTagIncluded(unitController.tag, Damager.damagingTags) && defencer)
					{
						DamageManager.Instance.ExecuteDamage(Damager, defencer, Attacker);
					}
				}
			}).UnRegisterWhenGameObjectDestroyed(this);

			MoveController.SPD.RegisterWithInitValue(value =>
			{
				SelfNavAgent.speed = value;
			}).UnRegisterWhenGameObjectDestroyed(this);
		}

		private void OnEnable()
		{
			// spawn experience ball
			Defencer.OnDeath += damage =>
			{
				Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, 0);
				ExpGenerator.Instance.GenerateExpBall(EXPValue.Value, spawnPosition);
				onDestroy?.Invoke();
			};
		}

		private void Update()
		{
			if (target && SelfNavAgent.isOnNavMesh)
			{
				SelfNavAgent.SetDestination(target.position);
			}
		}
	}
}
