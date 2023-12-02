// Generate Id:c56a915b-e3f2-4db8-912d-d6bf20078991
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
