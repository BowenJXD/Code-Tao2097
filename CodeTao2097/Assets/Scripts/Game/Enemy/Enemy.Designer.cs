// Generate Id:b6617973-1c6c-4919-9617-11ea6726448f
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
