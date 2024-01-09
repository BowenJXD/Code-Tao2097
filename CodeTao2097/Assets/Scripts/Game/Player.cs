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
			Defencer.TakeDamageAfter += (damage) =>
			{
				Sprite.color = damage.DamageElement.GetColor();

				ActionKit.Delay(Defencer.DMGCD, () =>
				{
					if (!this) return;
					Sprite.color = Color.white;
				}).Start(this);
			};
			
			Defencer.OnDeath += (damage) =>
			{
				Deinit();
			};

			MoveController.MovementDirection.RegisterWithInitValue(value =>
			{
				anim.SetBool("isMoving", value != Vector2.zero);
				anim.SetFloat("moveX", value.x);
				anim.SetFloat("moveY", value.y);
			}).UnRegisterWhenGameObjectDestroyed(this);
			
			SelfAttributeController.onAddAAtModGroup += AddAAtMod;
			SelfAttributeController.onAddWAtModGroup += Inventory.AddWAtModGroup;
		}

		void Update()
		{
		    MoveController.MovementDirection.Value = GetMovementDirection();
		    Move(MoveController.MovementDirection.Value);
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

		public override BindableStat GetAAtMod(EAAt at)
		{
			BindableStat result = null;
			switch (at)
            {
                case EAAt.ATK:
                    result = Attacker.ATK;
                    break;
                case EAAt.CritRate:
                    result = Attacker.CritRate;
                    break;
                case EAAt.CritDamage:
                    result = Attacker.CritDamage;
                    break;
                case EAAt.AllElementBON:
                    result = Attacker.ElementBonuses[ElementType.All];
                    break;
                case EAAt.MetalElementBON:
                    result = Attacker.ElementBonuses[ElementType.Metal];
                    break;
                case EAAt.WoodElementBON:
                    result = Attacker.ElementBonuses[ElementType.Wood];
                    break;
                case EAAt.WaterElementBON:
                    result = Attacker.ElementBonuses[ElementType.Water];
                    break;
                case EAAt.FireElementBON:
                    result = Attacker.ElementBonuses[ElementType.Fire];
                    break;
                case EAAt.EarthElementBON:
                    result = Attacker.ElementBonuses[ElementType.Earth];
                    break;
                
                case EAAt.DEF:
                    result = Defencer.DEF;
                    break;
                case EAAt.MaxHP:
                    result = Defencer.MaxHP;
                    break;
                case EAAt.AllElementRES:
                    result = Defencer.ElementResistances[ElementType.All];
                    break;
                case EAAt.MetalElementRES:
                    result = Defencer.ElementResistances[ElementType.Metal];
                    break;
                case EAAt.WoodElementRES:
                    result = Defencer.ElementResistances[ElementType.Wood];
                    break;
                case EAAt.WaterElementRES:
                    result = Defencer.ElementResistances[ElementType.Water];
                    break;
                case EAAt.FireElementRES:
                    result = Defencer.ElementResistances[ElementType.Fire];
                    break;
                case EAAt.EarthElementRES:
                    result = Defencer.ElementResistances[ElementType.Earth];
                    break;
                
                case EAAt.SPD:
                    result = MoveController.SPD;
                    break;
                
                case EAAt.EXPBonus:
                    result = ExpController.EXPRate;
                    break;
                
                default:
                    break;
            }

			return result;
		}
	}
}
