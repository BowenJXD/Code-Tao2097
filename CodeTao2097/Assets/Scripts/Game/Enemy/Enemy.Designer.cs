// Generate Id:d04b6b0b-bbca-44b1-91b6-3e6cbe72c642
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

		public Buffs.BuffOwner SelfBuffOwner;

		public UnityEngine.Rigidbody2D SelfRigidbody2D;

		public UnityEngine.AI.NavMeshAgent SelfNavAgent;

	}
}
