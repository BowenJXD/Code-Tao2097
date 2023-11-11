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
			SelfNavAgent.speed = SPD;
		}
	}
}
