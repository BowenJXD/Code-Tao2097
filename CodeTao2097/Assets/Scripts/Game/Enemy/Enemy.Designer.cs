// Generate Id:693ccac6-67a8-41cc-84f7-c8006f36ba53
using UnityEngine;

namespace CodeTao
{
	public partial class Enemy
	{

		public SpriteRenderer Sprite;

		public CircleCollider2D HitBox;

		public CodeTao.Attacker Attacker;

		public CodeTao.Defencer Defencer;

		public CodeTao.Damager Damager;

		public UnityEngine.Rigidbody2D SelfRigidbody2D;

		public UnityEngine.AI.NavMeshAgent SelfNavAgent;

	}
}
