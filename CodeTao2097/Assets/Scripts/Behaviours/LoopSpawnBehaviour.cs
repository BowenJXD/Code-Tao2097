using System;
using QFramework;
using UnityEngine.Serialization;

namespace CodeTao
{
    public class LoopSpawnBehaviour : SpawnBehaviour, IWAtReceiver
    {
        public BindableStat spawnInterval;
        LoopTask spawnTask;

        private void OnEnable()
        {
            spawnTask = new LoopTask(this, spawnInterval, Spawn);
            spawnInterval.RegisterWithInitValue( value => spawnTask.LoopInterval = value).
                UnRegisterWhenGameObjectDestroyed(this);
            spawnTask.Start();
        }

        private void OnDisable()
        {
            spawnTask?.Finish();
            spawnTask = null;
        }

        public void Receive(IWAtSource source)
        {
            spawnInterval.InheritStat(source.GetWAt(EWAt.Cooldown));
        }
    }
}