using System;
using GraphProcessor;
using UnityEngine;
using QFramework;
using UnityEngine.Serialization;

namespace CodeTao
{
	public partial class Enemy : CombatUnit
	{
		public Transform target;
		public BindableStat EXPValue = new BindableStat(1);

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
					if (Util.IsTagIncluded(unitController.tag, EnemyDamager.damagingTags) && defencer)
					{
						DamageManager.Instance.ExecuteDamage(EnemyDamager, defencer, Attacker);
					}
				}
			}).UnRegisterWhenGameObjectDestroyed(this);

			MoveController.SPD.RegisterWithInitValue(value =>
			{
				SelfNavAgent.speed = value;
			}).UnRegisterWhenGameObjectDestroyed(this);
			
			SelfAttributeController.onAddAAtModGroup += AddAAtMod;
		}

		private void OnEnable()
		{
			Defencer.Revive();
			// spawn experience ball
			Defencer.OnDeath += damage =>
			{
				Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, 0);
				ExpGenerator.Instance.GenerateExpBall(EXPValue.Value, spawnPosition);
				Deinit();
			};
		}

		private void Update()
		{
			if (target && SelfNavAgent.isOnNavMesh)
			{
				SelfNavAgent.SetDestination(target.position);
			}
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
                    EXPValue.AddModifierGroup(modGroup);
                    break;
                
                default:
                    break;
            }
        }
	}
}
