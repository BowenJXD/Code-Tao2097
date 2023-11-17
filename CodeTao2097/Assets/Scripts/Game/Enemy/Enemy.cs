using System;
using GraphProcessor;
using UnityEngine;
using QFramework;

namespace CodeTao
{
	public partial class Enemy : MoveController
	{
		private void Start()
		{
			SelfNavAgent.updateRotation = false;
			SelfNavAgent.updateUpAxis = false;
		}

		private void Update()
		{
			if (Player.Instance)
			{
				SelfNavAgent.speed = SPD;
				SelfNavAgent.SetDestination(Player.Instance.transform.position);
			}
		}
	}
}
