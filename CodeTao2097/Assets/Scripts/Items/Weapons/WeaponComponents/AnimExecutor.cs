using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Pool;

namespace CodeTao
{
    public class AnimExecutor : WeaponExecutor
    {
        public AnimateObject spawnAnimation;
        protected ObjectPool<AnimateObject> aniPool;

        public override void Init(Weapon newWeapon)
        {
            base.Init(newWeapon);
            if (!spawnAnimation)
            {
                spawnAnimation = this.GetComponentInDescendants<AnimateObject>(true);
            }

            if (spawnAnimation){
                aniPool = new ObjectPool<AnimateObject>(
                    () => { return Instantiate(spawnAnimation, AnimateObjectManager.Instance.transform); }, 
                    prefab => { prefab.gameObject.SetActive(true); }, 
                    prefab => { prefab.gameObject.SetActive(false); }, 
                    prefab => { Destroy(prefab); });
            }
        }

        public override void Execute(List<Vector3> globalPositions)
        {
            base.Execute(globalPositions);
            foreach (var globalPos in globalPositions)
            {
                AnimateObject newAnim = aniPool.Get().Position(globalPos);
                newAnim.onTrigger = Next;
                newAnim.onEnd = () => { aniPool.Release(newAnim); };
            }
        }
    }
}