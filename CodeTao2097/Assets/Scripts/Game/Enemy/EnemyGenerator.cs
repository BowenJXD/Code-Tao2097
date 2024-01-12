using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CodeTao;
using UnityEngine;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CodeTao
{
	/// <summary>
	/// 敌人生成器，根据任务生成敌人。
	/// </summary>
	public partial class EnemyGenerator : MonoBehaviour
	{
		protected UnitPool<Enemy> enemyPool;
		public Enemy enemyPrefab;
		public List<GeneratorTask> tasks;
		
		protected ObjectPool<ParticleSystem> deathFXPool;
		public ParticleSystem deathFX;
		protected float deathFXDuration;
		
		public float minDistance = 10;
		public float maxDistance = 20;
		public float distributionVariationInterval = 5;
		public int minDistribution = 10;
		public float maxDistributionVariation = 20;
		
		protected float lastSpawnedAngle = 0;
		
		private void Awake()
		{
			enemyPool = new UnitPool<Enemy>(enemyPrefab, transform, 2000);

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
			deathFXDuration = deathFX.main.duration;
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
			
			Vector3 spawnPosition = GetSpawnPosition();
			Enemy enemy = enemyPool.Get().Position(spawnPosition).Parent(transform);
			enemy.onDeinit += () =>
			{
				enemyPool.Release(enemy);
				ParticleSystem ps = deathFXPool.Get().Position(enemy.transform.position).Parent(transform);
				ActionKit.Delay(deathFXDuration, () => { deathFXPool.Release(ps); }).Start(this);
			};

			enemy.Init();
		}

		Vector3 GetSpawnPosition()
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
			return spawnPosition;
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
