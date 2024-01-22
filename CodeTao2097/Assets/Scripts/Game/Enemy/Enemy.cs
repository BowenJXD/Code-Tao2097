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

		public override void PreInit()
		{
			base.PreInit();
			
			SelfNavAgent.updateRotation = false;
			SelfNavAgent.updateUpAxis = false;
			
			// Change color after taking DMG
			GetComp<Defencer>().TakeDamageAfter += (damage) =>
			{
				Sprite.color = damage.DamageElement.GetColor();

				ActionKit.Delay(0.1f, () =>
				{
					if (!this) return;
					Sprite.color = Color.white;
				}).Start(this);
			};

			// Attack player when player is in range
			HitBox.OnTriggerEnter2DEvent((col) =>
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
			});

			GetComp<MoveController>().SPD.RegisterWithInitValue(value =>
			{
				SelfNavAgent.speed = value;
			});
		}

		private void Start()
		{
			if (!target)
			{
				target = Player.Instance.transform;
			}
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
	}
}
