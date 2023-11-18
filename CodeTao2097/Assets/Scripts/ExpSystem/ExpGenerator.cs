using QFramework;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace CodeTao
{
    public class ExpPool : ObjectPool<ExpBall>
    {
        public ExpPool(ExpBall defaultPrefab, int defaultCapacity = 10) :
            base(() =>
                {
                    ExpBall expBall = Object.Instantiate(defaultPrefab);
                    return expBall;
                }, expBallPrefab =>
                {
                    expBallPrefab.collider2D.enabled = true;
                    expBallPrefab.gameObject.SetActive(true);
                }
                , expBallPrefab =>
                {
                    expBallPrefab.gameObject.SetActive(false);
                    expBallPrefab.ToggleCollider(false);
                }
                , expBallPrefab => { Object.Destroy(expBallPrefab); }
                , true, defaultCapacity)
        {
        }
    }
    
    public class ExpGenerator : MonoSingleton<ExpGenerator>
    {
        public ExpBall expBallPrefab;
        public ExpPool ExpPool;
        
        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            ExpPool expPool = new ExpPool(expBallPrefab);
        }

        public ExpBall GenerateExpBall(float expValue, Vector3 position)
        {
            ExpBall expBall = ExpPool.Get();
            expBall.EXPValue.Value = expValue;
            expBall.ToggleCollider(true);
            return expBall;
        }
    }
}