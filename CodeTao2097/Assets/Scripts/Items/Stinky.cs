using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public partial class Stinky : Weapon
    {
        public float radius;
		
        public LoopTask ShootTask;

        protected virtual void Start()
        {
            attacker = Player.Instance.Attacker;
            damager = Damager;
            
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
			
            foreach (var col in colliders)
            {
                Defencer defencer = Util.GetComponentInSiblings<Defencer>(col);
                if (defencer)
                {
                    Attack(defencer);
                }
            }
        }
    }
}