// Generate Id:7e61d5da-d558-4b0c-b414-d6166d7b2ea3
using UnityEngine;

namespace CodeTao
{
	public partial class Enemy
	{

		public SpriteRenderer Sprite;

		public CircleCollider2D HitBox;

		public CodeTao.MoveController MoveController;

		public CodeTao.Attacker Attacker;

		public CodeTao.Defencer Defencer;

		public CodeTao.Damager EnemyDamager;

		public CodeTao.ElementOwner ElementOwner;

		public CodeTao.BuffOwner SelfBuffOwner;

		public CodeTao.AttributeController SelfAttributeController;

		public UnityEngine.Rigidbody2D SelfRigidbody2D;

		public UnityEngine.AI.NavMeshAgent SelfNavAgent;

	}
}
