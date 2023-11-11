using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace QFramework
{
    public struct UpdateProgressEvent
    {
        public float progress;
    }
    public interface ISceneMgrSystem : ISystem
    {
        void AsyncLoad(string name, Action callback = null);
    }
    public class SceneMgrSystem : AbstractSystem, ISceneMgrSystem
    {
        private IEnumerator _AsyncLoad(string name, Action callback)
        {
            var e = new UpdateProgressEvent();
            var ao = SceneManager.LoadSceneAsync(name);            
            while (!ao.isDone)
            {
                e.progress = ao.progress;
                this.SendEvent(e);
                yield return ao.progress;
            }            
            callback?.Invoke();
        }
        void ISceneMgrSystem.AsyncLoad(string name, Action callback)
        {            
            PublicMono.Instance.StartCoroutine(_AsyncLoad(name, callback));
        }
        protected override void OnInit() { }
    }
}