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
	/// TODO: 生成位置波动随时间变化，而不是按次数
	/// </summary>
	public partial class EnemyGenerator : MonoBehaviour
	{
		public Enemy enemyPrefab;
		public List<GeneratorTask> tasks;
		[NonSerialized] public List<Enemy> activeEnemies = new List<Enemy>();
		
		protected ObjectPool<ParticleSystem> deathFXPool;
		public ParticleSystem deathFX;
		protected float deathFXDuration;
		
		// 生成位置波动
		public float minDistance = 10;
		public float maxDistance = 20;
		public float distributionVariationInterval = 5;
		public int minDistribution = 10;
		public float maxDistributionVariation = 20;
		protected float lastSpawnedAngle = 0;

		public Action<Enemy> onSpawn;
		
		private void Awake()
		{
			UnitManager.Instance.Register(enemyPrefab, transform, 2000);

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
			Enemy enemy = UnitManager.Instance.Get(enemyPrefab);
			activeEnemies.Add(enemy);
			enemy.Position(spawnPosition);
			onSpawn?.Invoke(enemy);
			enemy.onDeinit += () =>
			{
				ParticleSystem ps = deathFXPool.Get().Position(enemy.transform.position).Parent(transform);
				ActionKit.Delay(deathFXDuration, () => { deathFXPool.Release(ps); }).Start(this);
				activeEnemies.Remove(enemy);
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

		private void OnDisable()
		{
			onSpawn = null;
		}
	}
}
