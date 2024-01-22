using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 每走一段路发动一次冲击。
    /// </summary>
    public class MoveFireExecutor : WeaponExecutor
    {
        private Rigidbody2D rb;
        public float totalDistance;
        private float distance = 0;
        
        public override void Init(Weapon newWeapon)
        {
            base.Init(newWeapon);
            rb = weapon.Container.Unit.GetComponent<Rigidbody2D>();
        }

        public override IEnumerator ExecuteCoroutine(List<Vector3> globalPositions)
        {
            while (rb && distance < totalDistance)
            {
                distance += rb.velocity.magnitude * Time.deltaTime;
                yield return new WaitForNextFrameUnit();
            }
            distance = 0;
        }
    }
}