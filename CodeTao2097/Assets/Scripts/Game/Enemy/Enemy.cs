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
	                result = EXPValue;
                    break;
                
                default:
                    break;
            }

			return result;
		}
	}
}
