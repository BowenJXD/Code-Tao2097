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
            
            moveSpeed.RegisterWithInitValue(value =>
            {
                SelfNavAgent.speed = value;
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        private void OnEnable()
        {
            SelfNavAgent.updateRotation = false;
            SelfNavAgent.updateUpAxis = false;
            SelfNavAgent.enabled = false;
            
            col2D = HitBox;
            
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
                target = col.transform;
            };
        }

        public void ToggleCollider(bool value)
        {
            col2D.enabled = value;
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
            SelfNavAgent.speed = moveSpeed.Value;
            ValidateCollectFunc = col =>
            {
                if (!ComponentUtil.GetComponentInAncestors<Defencer>(col))
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
            if (SelfNavAgent.enabled && target)
            {
                SelfNavAgent.SetDestination(target.position);
            }
        }
    }
}