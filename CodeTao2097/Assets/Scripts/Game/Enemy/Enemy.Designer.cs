// Generate Id:3ce19a03-197d-4fc0-972b-f7d37d2a1665
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

		public CodeTao.Damager Damager;

		public CodeTao.ElementOwner ElementOwner;

		public UnityEngine.Rigidbody2D SelfRigidbody2D;

		public UnityEngine.AI.NavMeshAgent SelfNavAgent;

	}
}
