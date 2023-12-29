using System;
using UnityEngine;
using QFramework;

namespace CodeTao
{
	public partial class Player : CombatUnit
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
				Deinit();
			};
			
			SelfAttributeController.onAddAAtModGroup += AddAAtMod;
			SelfAttributeController.onAddWAtModGroup += Inventory.AddWAtModGroup;
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

		public override void AddAAtMod(EAAt at, ModifierGroup modGroup)
		{
			base.AddAAtMod(at, modGroup);
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
