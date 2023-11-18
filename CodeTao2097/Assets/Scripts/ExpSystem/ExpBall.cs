using System;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public partial class ExpBall : Collectable
    {
        public BindableProperty<float> EXPValue = new BindableProperty<float>(1);

        public BindableProperty<float> moveSpeed;
        public Transform target;
        
        protected override void Start()
        {
            base.Start();
            collider2D = GetComponent<Collider2D>();
            
            SelfNavAgent.updateRotation = false;
            SelfNavAgent.updateUpAxis = false;
            
            moveSpeed.RegisterWithInitValue(value =>
            {
                SelfNavAgent.speed = value;
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        private void OnEnable()
        {
            SelfNavAgent.enabled = false;
            
            ValidateCollectFunc = col =>
            {
                if (!ComponentUtil.GetComponentInAncestors<Collector>(col))
                {
                    return false;
                }

                return true;
            };
            
            CollectFunc = col =>
            {
                StartChase();
            };
        }

        public void ToggleCollider(bool value)
        {
            collider2D.enabled = value;
        }
        
        public Func<Collider2D, bool> ValidateCollectFunc;
        
        public override bool ValidateCollect(Collider2D col)
        {
            bool result = base.ValidateCollect(col);
            if (ValidateCollectFunc != null)
            {
                if (!ValidateCollectFunc(col))
                {
                    result = false;
                }
            }
            return result;
        }

        public Action<Collider2D> CollectFunc;
        
        public override void Collect(Collider2D collector = null)
        {
            CollectFunc?.Invoke(collector);
        }

        protected void StartChase()
        {
            SelfNavAgent.enabled = true;
            ValidateCollectFunc = col =>
            {
                if (!ComponentUtil.GetComponentInAncestors<Damager>(col))
                {
                    return false;
                }

                return true;
            };
            CollectFunc = col =>
            {
                UnitController unitController = ComponentUtil.GetComponentInAncestors<UnitController>(col);
                ExpController expController = ComponentUtil.GetComponentInDescendants<ExpController>(unitController);
                expController.EXP.Value += EXPValue.Value;
                Destroy(gameObject);
            };
        }

        private void OnDisable()
        {
            ValidateCollectFunc = null;
            CollectFunc = null;
        }

        private void Update()
        {
            if (SelfNavAgent.enabled)
            {
                SelfNavAgent.SetDestination(target.position);
            }
        }
    }
}