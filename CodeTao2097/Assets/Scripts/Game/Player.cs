using System;
using UnityEngine;
using QFramework;

namespace CodeTao
{
	/// <summary>
	/// 玩家单位，用于管理玩家专属的多组件交互逻辑，和控制动画。
	/// </summary>
	public partial class Player : CombatUnit
	{
		public static Player Instance;
		public Animator anim;

		private void Awake()
		{
			Instance = this;
			anim = this.GetComponentInDescendants<Animator>();
		}

		private void Start()
		{
			// Change color after taking DMG
			GetComp<Defencer>().TakeDamageAfter += (damage) =>
			{
				Sprite.color = damage.DamageElement.GetColor();

				ActionKit.Delay(GetComp<Defencer>().DMGCD, () =>
				{
					if (!this) return;
					Sprite.color = Color.white;
				}).Start(this);
			};
			
			GetComp<Defencer>().OnDeath += (damage) =>
			{
				Deinit();
			};

			GetComp<MoveController>().movementDirection.RegisterWithInitValue(value =>
			{
				anim.SetBool("isMoving", value != Vector2.zero);
				anim.SetFloat("moveX", value.x);
				anim.SetFloat("moveY", value.y);
			}).UnRegisterWhenGameObjectDestroyed(this);
		}

		void Update()
		{
			GetComp<MoveController>().movementDirection.Value = GetMovementDirection();
		}

		public Vector2 GetMovementDirection()
		{
			var horizontal = Input.GetAxis("Horizontal");
			var vertical = Input.GetAxis("Vertical");
			return new Vector2(horizontal, vertical).normalized;
		}
	}
}
