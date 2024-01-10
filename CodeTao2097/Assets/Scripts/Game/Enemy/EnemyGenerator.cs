using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeTao;
using UnityEngine;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CodeTao
{
	/// <summary>
	/// 敌人生成器，根据任务生成敌人。
	/// </summary>
	public partial class EnemyGenerator : ViewController
	{
		protected List<UnitPool<Enemy>> enemyPools = new List<UnitPool<Enemy>>();
		public Enemy enemyPrefab;
		public List<GeneratorTask> tasks;
		
		protected ObjectPool<ParticleSystem> deathFXPool;
		public ParticleSystem deathFX;
		
		public float minDistance = 10;
		public float maxDistance = 20;
		public float distributionVariationInterval = 5;
		public int minDistribution = 10;
		public float maxDistributionVariation = 20;
		
		protected float lastSpawnedAngle = 0;
		
		private void Awake()
		{
			UnitPool<Enemy> enemyPool = new UnitPool<Enemy>(enemyPrefab, transform, 2000);
			enemyPools.Add(enemyPool);
			
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
			, true, 100);
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
				tasks.RemoveAt(0);
				task.Start(this);
				task.onGenerate.AddListener(ProcessTask);
				task.onFinish.AddListener(NewTask);
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
				float randomRange = maxDistributionVariation 
					* Mathf.Cos(Global.GameTime / distributionVariationInterval);
				int roundedAbsRange = Mathf.RoundToInt(Mathf.Abs(randomRange));
				int distribution = minDistribution + RandomUtil.Rand(roundedAbsRange);
				distribution *= RandomUtil.RandBool()? 1: -1;
				lastSpawnedAngle += distribution;
				Vector3 spawnDirection = Quaternion.Euler(0f, 0f, lastSpawnedAngle) * Vector2.right;
				Vector3 spawnPosition = Player.Instance.transform.position + spawnDirection * randomDistance;
				
				Enemy enemy = enemyPools[index].Get().Position(spawnPosition).Parent(transform);
				enemy.onDeinit += () =>
				{
					enemyPools[index].Release(enemy);
					ParticleSystem ps = deathFXPool.Get().Position(enemy.transform.position).Parent(transform);
					ActionKit.Delay(ps.main.duration, () => { deathFXPool.Release(ps); }).Start(this);
				};
				enemy.Init();
			}
			else
			{
				Debug.LogError($"Index {index} / {enemyPools.Count} is out of range!");
			}
		}
		
		public void Pause()
		{
			foreach (GeneratorTask task in tasks)
			{
				task.LoopTask.Pause();
			}
		}
		
		public void Resume()
		{
			foreach (GeneratorTask task in tasks)
			{
				task.LoopTask.Resume();
			}
		}
	}
}
