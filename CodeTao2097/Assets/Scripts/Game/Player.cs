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
            switch (at)
            {
                case EAAt.ATK:
                    Attacker.ATK.AddModifierGroup(modGroup);
                    break;
                case EAAt.CritRate:
                    Attacker.CritRate.AddModifierGroup(modGroup);
                    break;
                case EAAt.CritDamage:
                    Attacker.CritDamage.AddModifierGroup(modGroup);
                    break;
                case EAAt.AllElementBON:
                    Attacker.ElementBonuses[ElementType.All].AddModifierGroup(modGroup);
                    break;
                case EAAt.MetalElementBON:
                    Attacker.ElementBonuses[ElementType.Metal].AddModifierGroup(modGroup);
                    break;
                case EAAt.WoodElementBON:
                    Attacker.ElementBonuses[ElementType.Wood].AddModifierGroup(modGroup);
                    break;
                case EAAt.WaterElementBON:
                    Attacker.ElementBonuses[ElementType.Water].AddModifierGroup(modGroup);
                    break;
                case EAAt.FireElementBON:
                    Attacker.ElementBonuses[ElementType.Fire].AddModifierGroup(modGroup);
                    break;
                case EAAt.EarthElementBON:
                    Attacker.ElementBonuses[ElementType.Earth].AddModifierGroup(modGroup);
                    break;
                
                case EAAt.DEF:
                    Defencer.DEF.AddModifierGroup(modGroup);
                    break;
                case EAAt.MaxHP:
                    Defencer.MaxHP.AddModifierGroup(modGroup);
                    break;
                case EAAt.AllElementRES:
                    Defencer.ElementResistances[ElementType.All].AddModifierGroup(modGroup);
                    break;
                case EAAt.MetalElementRES:
                    Defencer.ElementResistances[ElementType.Metal].AddModifierGroup(modGroup);
                    break;
                case EAAt.WoodElementRES:
                    Defencer.ElementResistances[ElementType.Wood].AddModifierGroup(modGroup);
                    break;
                case EAAt.WaterElementRES:
                    Defencer.ElementResistances[ElementType.Water].AddModifierGroup(modGroup);
                    break;
                case EAAt.FireElementRES:
                    Defencer.ElementResistances[ElementType.Fire].AddModifierGroup(modGroup);
                    break;
                case EAAt.EarthElementRES:
                    Defencer.ElementResistances[ElementType.Earth].AddModifierGroup(modGroup);
                    break;
                
                case EAAt.SPD:
                    MoveController.SPD.AddModifierGroup(modGroup);
                    break;
                
                case EAAt.EXPBonus:
                    ExpController.EXPRate.AddModifierGroup(modGroup);
                    break;
                
                default:
                    break;
            }
        }
	}
}
