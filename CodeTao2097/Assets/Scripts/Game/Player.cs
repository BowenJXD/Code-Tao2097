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

			GetComp<MoveController>().MovementDirection.RegisterWithInitValue(value =>
			{
				anim.SetBool("isMoving", value != Vector2.zero);
				anim.SetFloat("moveX", value.x);
				anim.SetFloat("moveY", value.y);
			}).UnRegisterWhenGameObjectDestroyed(this);
			
			GetComp<AttributeController>().onAddAAtModGroup += AddAAtMod;
			GetComp<AttributeController>().onAddWAtModGroup += GetComp<Inventory>().AddWAtModGroup;
		}

		void Update()
		{
			GetComp<MoveController>().MovementDirection.Value = GetMovementDirection();
		    Move(GetComp<MoveController>().MovementDirection.Value);
		}

		public Vector2 GetMovementDirection()
		{
			var horizontal = Input.GetAxis("Horizontal");
			var vertical = Input.GetAxis("Vertical");
			return new Vector2(horizontal, vertical).normalized;
		}
		
		public void Move(Vector2 direction)
		{
			SelfRigidbody2D.velocity = direction * GetComp<MoveController>().SPD;
		}

		public override BindableStat GetAAtMod(EAAt at)
		{
			BindableStat result = null;
			switch (at)
            {
                case EAAt.ATK:
                    result = GetComp<Attacker>().ATK;
                    break;
                case EAAt.CritRate:
                    result = GetComp<Attacker>().CritRate;
                    break;
                case EAAt.CritDamage:
                    result = GetComp<Attacker>().CritDamage;
                    break;
                case EAAt.AllElementBON:
                    result = GetComp<Attacker>().ElementBonuses[ElementType.All];
                    break;
                case EAAt.MetalElementBON:
                    result = GetComp<Attacker>().ElementBonuses[ElementType.Metal];
                    break;
                case EAAt.WoodElementBON:
                    result = GetComp<Attacker>().ElementBonuses[ElementType.Wood];
                    break;
                case EAAt.WaterElementBON:
                    result = GetComp<Attacker>().ElementBonuses[ElementType.Water];
                    break;
                case EAAt.FireElementBON:
                    result = GetComp<Attacker>().ElementBonuses[ElementType.Fire];
                    break;
                case EAAt.EarthElementBON:
                    result = GetComp<Attacker>().ElementBonuses[ElementType.Earth];
                    break;
                
                case EAAt.DEF:
                    result = GetComp<Defencer>().DEF;
                    break;
                case EAAt.MaxHP:
                    result = GetComp<Defencer>().MaxHP;
                    break;
                case EAAt.Lives:
					result = GetComp<Defencer>().Lives;
					break;
                
                case EAAt.AllElementRES:
                    result = GetComp<Defencer>().ElementResistances[ElementType.All];
                    break;
                case EAAt.MetalElementRES:
                    result = GetComp<Defencer>().ElementResistances[ElementType.Metal];
                    break;
                case EAAt.WoodElementRES:
                    result = GetComp<Defencer>().ElementResistances[ElementType.Wood];
                    break;
                case EAAt.WaterElementRES:
                    result = GetComp<Defencer>().ElementResistances[ElementType.Water];
                    break;
                case EAAt.FireElementRES:
                    result = GetComp<Defencer>().ElementResistances[ElementType.Fire];
                    break;
                case EAAt.EarthElementRES:
                    result = GetComp<Defencer>().ElementResistances[ElementType.Earth];
                    break;
                
                case EAAt.SPD:
                    result = GetComp<MoveController>().SPD;
                    break;
                
                case EAAt.EXPBonus:
                    result = GetComp<ExpController>().EXPRate;
                    break;
                
                default:
                    break;
            }

			return result;
		}
	}
}
