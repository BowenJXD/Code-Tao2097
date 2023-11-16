using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class Stinky : Weapon
    {
        public float radius;
		
        public LoopTask ShootTask;

        void Start()
        {
            ShootTask = new LoopTask(this, attackInterval, AttackSurrounding);
            ShootTask.Start();
            attackInterval.RegisterWithInitValue(interval =>
            {
                ShootTask.LoopInterval = interval;
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public void AttackSurrounding()
        {
            List<Collider2D> colliders = Physics2D.OverlapCircleAll(transform.position, radius).ToList();
			
            foreach (var collider in colliders)
            {
                Defencer defencer = collider.GetComponentInParent<Defencer>();
                if (defencer)
                {
                    DamageManager.Instance.ExecuteDamage(damager, defencer, attacker);
                }
            }
        }
    }
}