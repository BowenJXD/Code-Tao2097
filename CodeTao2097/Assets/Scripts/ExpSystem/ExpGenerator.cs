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
                    expBallPrefab.gameObject.SetActive(true);
                    expBallPrefab.ToggleCollider(true);
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
    
    public partial class ExpGenerator : MonoSingleton<ExpGenerator>
    {
        public ExpBall expBallPrefab;
        public ExpPool ExpPool;
        
        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            ExpPool = new ExpPool(expBallPrefab);
        }

        public void GenerateExpBall(float expValue, Vector3 position)
        {
            ActionKit.DelayFrame(1, () =>
            {
                ExpBall expBall = ExpPool.Get();
                expBall.transform.position = position;
                expBall.transform.parent = transform;
                expBall.EXPValue.Value = expValue;
                expBall.ToggleCollider(true);
            }).Start(this);
        }
    }
}