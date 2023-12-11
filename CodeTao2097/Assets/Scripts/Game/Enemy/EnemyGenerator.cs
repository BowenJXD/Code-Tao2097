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

namespace CodeTao
{
	public partial class EnemyGenerator : ViewController
	{
		protected List<UnitPool<Enemy>> enemyPools = new List<UnitPool<Enemy>>();
		public List<Enemy> enemyPrefabs = new List<Enemy>();
		public List<GeneratorTask> tasks = new List<GeneratorTask>();
		
		protected ObjectPool<ParticleSystem> deathFXPool;
		public ParticleSystem deathFX;
		
		public float minDistance = 10;
		public float maxDistance = 20;
		
		private void Awake()
		{
			for (int i = 0; i < enemyPrefabs.Count; i++)
			{
				UnitPool<Enemy> enemyPool = new UnitPool<Enemy>(enemyPrefabs[i]);
				enemyPools.Add(enemyPool);
			}
			
			deathFXPool = new ObjectPool<ParticleSystem>(() =>
			{
				ParticleSystem particleSystem = Instantiate(deathFX);
				return particleSystem;
			}, prefab =>
			{
				prefab.gameObject.SetActive(true);
				prefab.Play();
			}
			, prefab =>
			{
				prefab.gameObject.SetActive(false);
				prefab.Stop();
				prefab.Clear();
			}
			, prefab => { Destroy(prefab); }
			, true, 4);
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
			if (index < enemyPools.Count)
			{
				float randomDistance = Random.Range(minDistance, maxDistance);
				float randomAngle = Random.Range(0f, 360f);
				Vector3 spawnDirection = Quaternion.Euler(0f, 0f, randomAngle) * Vector2.right;
				Vector3 spawnPosition = Player.Instance.transform.position + spawnDirection * randomDistance;
				
				Enemy enemy = enemyPools[index].Get().Position(spawnPosition).Parent(transform);
				enemy.onDestroy += () =>
				{
					enemyPools[index].Release(enemy);
					deathFXPool.Get().Position(enemy.transform.position).Parent(transform);
				};
			}
			else
			{
				Debug.LogError($"Index {index} / {enemyPools.Count} is out of range!");
			}
		}
	}
}
