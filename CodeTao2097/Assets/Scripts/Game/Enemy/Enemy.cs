using System;
using GraphProcessor;
using UnityEngine;
using QFramework;
using UnityEngine.Serialization;

namespace CodeTao
{
	/// <summary>
	/// 敌人，通过自动寻路追击玩家，在触碰玩家时造成伤害。
	/// </summary>
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
			GetComp<Defencer>().TakeDamageAfter += (damage) =>
			{
				Sprite.color = damage.DamageElement.GetColor();

				ActionKit.Delay(GetComp<Defencer>().DMGCD, () =>
				{
					if (!this) return;
					Sprite.color = Color.white;
				}).Start(this);
			};

			// Attack player when player is in range
			HitBox.OnTriggerStay2DEvent((col) =>
			{
				UnitController unitController = col.GetComponentInAncestors<UnitController>();
				Defencer def = col.GetComponentInAncestors<Defencer>(1);
				if (unitController)
				{
					if (Util.IsTagIncluded(unitController.tag, GetComp<Damager>().damagingTags) && def)
					{
						DamageManager.Instance.ExecuteDamage(GetComp<Damager>(), def, GetComp<Attacker>());
					}
				}
			}).UnRegisterWhenGameObjectDestroyed(this);

			GetComp<MoveController>().SPD.RegisterWithInitValue(value =>
			{
				SelfNavAgent.speed = value;
			}).UnRegisterWhenGameObjectDestroyed(this);
			
			GetComp<AttributeController>().onAddAAtModGroup += AddAAtMod;
		}

		private void OnEnable()
		{
			// spawn experience ball
			GetComp<Defencer>().OnDeath += damage =>
			{
				Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, 0);
				var expBall = ExpGenerator.Instance.Get().Position(spawnPosition);
				expBall.EXPValue.Value = EXPValue.Value;
				expBall.Init();
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
	                result = EXPValue;
                    break;
                
                default:
                    break;
            }

			return result;
		}
	}
}
