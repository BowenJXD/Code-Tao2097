using UnityEngine;
using QFramework;

namespace CodeTao
{
	public partial class Player : MoveController
	{
		void Update()
		{
			Move(GetMovementDirection());
		}

		public Vector2 GetMovementDirection()
		{
			var horizontal = Input.GetAxis("Horizontal");
			var vertical = Input.GetAxis("Vertical");
			return new Vector2(horizontal, vertical).normalized;
		}
		
		public void Move(Vector2 direction)
		{
			SelfRigidbody2D.velocity = direction * SPD;
		}
	}
}
