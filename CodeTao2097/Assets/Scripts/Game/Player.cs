using System;
using UnityEngine;
using QFramework;

namespace CodeTao
{
	public partial class Player : UnitController
	{
		void Update()
		{
			MoveController.MovementDirection.Value = GetMovementDirection();
			Move(MoveController.MovementDirection.Value);
		}

		public static Player Instance;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
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
			
			Defencer.OnDeath += (damage) =>
			{
				onDestroy?.Invoke();
			};
			
			Inventory.AddAfter += (content) =>
			{
				Weapon weapon = (Weapon) content;
				if (weapon)
				{
					weapon.attacker = Attacker;
				}
			};
		}

		public Vector2 GetMovementDirection()
		{
			var horizontal = Input.GetAxis("Horizontal");
			var vertical = Input.GetAxis("Vertical");
			return new Vector2(horizontal, vertical).normalized;
		}
		
		public void Move(Vector2 direction)
		{
			SelfRigidbody2D.velocity = direction * MoveController.SPD;
		}
	}
}
