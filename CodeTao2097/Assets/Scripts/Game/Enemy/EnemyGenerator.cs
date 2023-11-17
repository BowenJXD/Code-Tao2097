using System;
using System.Collections;
using System.Collections.Generic;
using CodeTao;
using UnityEngine;
using QFramework;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace VSLTut
{
	public class EnemyPool : ObjectPool<Enemy>
	{
		public EnemyPool(Enemy defaultPrefab, int defaultCapacity = 10) :
			base(() =>
				{
					Enemy enemy = Object.Instantiate(defaultPrefab);
					return enemy;
				}, enemyPrefab =>
				{
					enemyPrefab.gameObject.SetActive(true);
				}
				, enemyPrefab => { enemyPrefab.gameObject.SetActive(false); }
				, enemyPrefab => { Object.Destroy(enemyPrefab); }
				, true, defaultCapacity)
		{
		}
	}

	public partial class EnemyGenerator : ViewController
	{
		public List<EnemyPool> EnemyPools = new List<EnemyPool>();
		public List<Enemy> enemyPrefabs = new List<Enemy>();
		public List<GeneratorTask> tasks = new List<GeneratorTask>();
		
		public float minDistance = 10;
		public float maxDistance = 20;
		
		private void Awake()
		{
			for (int i = 0; i < enemyPrefabs.Count; i++)
			{
				EnemyPool enemyPool = new EnemyPool(enemyPrefabs[i]);
				EnemyPools.Add(enemyPool);
			}
		}

		void Start()
		{
			NewTask();
		}

		public void NewTask()
		{
			if (tasks.Count > 0)
			{
				GeneratorTask task = tasks[0];
				task.Start(this);
				task.onGenerate.AddListener(ProcessTask);
				task.onFinish.AddListener(() =>
				{
					tasks.RemoveAt(0);
					NewTask();
				});
			}
			else
			{
				// Global.IsPass = true;
			}
		}

		public void ProcessTask(int index)
		{
			if (!Player.Instance) return;
			if (index < EnemyPools.Count)
			{
				float randomDistance = Random.Range(minDistance, maxDistance);
				float randomAngle = Random.Range(0f, 360f);
				Vector3 spawnDirection = Quaternion.Euler(0f, 0f, randomAngle) * Vector2.right;
				Vector3 spawnPosition = Player.Instance.transform.position + spawnDirection * randomDistance;
				
				Enemy enemy = EnemyPools[index].Get().Position(spawnPosition).Parent(transform).Show();
				enemy.Defencer.OnDeath += ((damage) =>
				{
					EnemyPools[index].Release(enemy);
				});
			}
			else
			{
				Debug.LogError($"Index {index} / {EnemyPools.Count} is out of range!");
			}
		}
	}
}
