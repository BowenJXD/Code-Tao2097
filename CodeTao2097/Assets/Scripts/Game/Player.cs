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
			Link.GetComp<Defencer>().TakeDamageAfter += (damage) =>
			{
				Sprite.color = damage.DamageElement.GetColor();

				ActionKit.Delay(Link.GetComp<Defencer>().DMGCD, () =>
				{
					if (!this) return;
					Sprite.color = Color.white;
				}).Start(this);
			};
			
			Link.GetComp<Defencer>().OnDeath += (damage) =>
			{
				Deinit();
			};

			Link.GetComp<MoveController>().MovementDirection.RegisterWithInitValue(value =>
			{
				anim.SetBool("isMoving", value != Vector2.zero);
				anim.SetFloat("moveX", value.x);
				anim.SetFloat("moveY", value.y);
			}).UnRegisterWhenGameObjectDestroyed(this);
			
			Link.GetComp<AttributeController>().onAddAAtModGroup += AddAAtMod;
			Link.GetComp<AttributeController>().onAddWAtModGroup += Link.GetComp<Inventory>().AddWAtModGroup;
		}

		void Update()
		{
			Link.GetComp<MoveController>().MovementDirection.Value = GetMovementDirection();
		    Move(Link.GetComp<MoveController>().MovementDirection.Value);
		}

		public Vector2 GetMovementDirection()
		{
			var horizontal = Input.GetAxis("Horizontal");
			var vertical = Input.GetAxis("Vertical");
			return new Vector2(horizontal, vertical).normalized;
		}
		
		public void Move(Vector2 direction)
		{
			SelfRigidbody2D.velocity = direction * Link.GetComp<MoveController>().SPD;
		}

		public override BindableStat GetAAtMod(EAAt at)
		{
			BindableStat result = null;
			switch (at)
            {
                case EAAt.ATK:
                    result = Link.GetComp<Attacker>().ATK;
                    break;
                case EAAt.CritRate:
                    result = Link.GetComp<Attacker>().CritRate;
                    break;
                case EAAt.CritDamage:
                    result = Link.GetComp<Attacker>().CritDamage;
                    break;
                case EAAt.AllElementBON:
                    result = Link.GetComp<Attacker>().ElementBonuses[ElementType.All];
                    break;
                case EAAt.MetalElementBON:
                    result = Link.GetComp<Attacker>().ElementBonuses[ElementType.Metal];
                    break;
                case EAAt.WoodElementBON:
                    result = Link.GetComp<Attacker>().ElementBonuses[ElementType.Wood];
                    break;
                case EAAt.WaterElementBON:
                    result = Link.GetComp<Attacker>().ElementBonuses[ElementType.Water];
                    break;
                case EAAt.FireElementBON:
                    result = Link.GetComp<Attacker>().ElementBonuses[ElementType.Fire];
                    break;
                case EAAt.EarthElementBON:
                    result = Link.GetComp<Attacker>().ElementBonuses[ElementType.Earth];
                    break;
                
                case EAAt.DEF:
                    result = Link.GetComp<Defencer>().DEF;
                    break;
                case EAAt.MaxHP:
                    result = Link.GetComp<Defencer>().MaxHP;
                    break;
                case EAAt.Lives:
					result = Link.GetComp<Defencer>().Lives;
					break;
                
                case EAAt.AllElementRES:
                    result = Link.GetComp<Defencer>().ElementResistances[ElementType.All];
                    break;
                case EAAt.MetalElementRES:
                    result = Link.GetComp<Defencer>().ElementResistances[ElementType.Metal];
                    break;
                case EAAt.WoodElementRES:
                    result = Link.GetComp<Defencer>().ElementResistances[ElementType.Wood];
                    break;
                case EAAt.WaterElementRES:
                    result = Link.GetComp<Defencer>().ElementResistances[ElementType.Water];
                    break;
                case EAAt.FireElementRES:
                    result = Link.GetComp<Defencer>().ElementResistances[ElementType.Fire];
                    break;
                case EAAt.EarthElementRES:
                    result = Link.GetComp<Defencer>().ElementResistances[ElementType.Earth];
                    break;
                
                case EAAt.SPD:
                    result = Link.GetComp<MoveController>().SPD;
                    break;
                
                case EAAt.EXPBonus:
                    result = Link.GetComp<ExpController>().EXPRate;
                    break;
                
                default:
                    break;
            }

			return result;
		}
	}
}
