using System.Collections.Generic;
using System.Linq;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 角度选择器，基于角度生成攻击的位置信息。
    /// </summary>
    public class RadialSelector : WeaponSelector
    {
        public EAimWay aimWay = EAimWay.Random;
        public float shootPointOffset = 1;
        public float minAngularRange = 0;
        public float maxAngularRange = 360;
        /// <summary>
        /// A list of angles in degrees, that will be added to the base direction, and keep enumerating.
        /// </summary>
        public List<float> shootingDirections = new List<float>();
        private int _currentDirectionIndex = 0;
        
        private MoveController _ownerMoveController;

        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            _ownerMoveController = weapon.Container.GetComp<MoveController>();
            
            if (shootingDirections.Count == 0)
            {
                weapon.amount.RegisterWithInitValue(value =>
                {
                    GenerateDirections(Mathf.RoundToInt(value));
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
        }

        public override Vector3 GetLocalPosition(int spawnIndex)
        {
            Vector2 basePoint = GetBaseDirection();
            float angle = Util.GetAngleFromVector(basePoint);
            if (shootingDirections.Count > 0)
            {
                _currentDirectionIndex += 1;
                _currentDirectionIndex %= shootingDirections.Count > 0 ? shootingDirections.Count : 1;
                angle += shootingDirections[_currentDirectionIndex];
            }
            else
            {
                float amount = weapon.amount.Value;
                angle += spawnIndex * 360 / amount;
            }
            
            return Util.GetVectorFromAngle(angle) * shootPointOffset;
        }

        public Vector2 GetBaseDirection()
        {
            Vector2 result = RandomUtil.GetRandomNormalizedVector();
            switch (aimWay)
            {
                case EAimWay.AutoTargeting:
                    var targets = weapon.GetTargets();
                    if (targets.Count > 0)
                    {
                        Defencer target = targets[0];
                        result = (target.transform.position - transform.position);
                    }
                    break;
                case EAimWay.Owner:
                    result = _ownerMoveController.lastNonZeroDirection.Value;
                    break;
                case EAimWay.Random:
                    result = RandomUtil.GetRandomNormalizedVector();
                    break;
                case EAimWay.Cursor:
                    result = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
                    break;
                case EAimWay.Sequential:
                    result = Vector2.right;
                    break;
            }

            return result.normalized;
        }
        
        [Button("Generate Directions")]
        public void GenerateDirections(int amount = 0)
        {
            if (amount == 0)
            {
                if (!weapon) weapon = GetComponent<Weapon>();
                if (!weapon) weapon = this.GetComponentInAncestors<Weapon>(1);
                weapon.amount.Init();
                amount = (int)weapon.amount.Value;
            }
            // if minAngularRange is 0, then step = maxAngularRange / amount
            float step = minAngularRange == 0? maxAngularRange / amount : Mathf.Min(minAngularRange, maxAngularRange / amount);
            shootingDirections = Util.GenerateAngles(amount, step);
        }
    }
}