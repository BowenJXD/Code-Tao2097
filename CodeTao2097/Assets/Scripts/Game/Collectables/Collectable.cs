using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 道具Collectable：修为、金币等。无实体。进入玩家的拾取范围内后，朝玩家移动。
    /// </summary>
    public class Collectable : Interactable
    {
        NavMeshAgent navAgent;
        MoveController moveController;
        Transform target;
        [HideInInspector] public Rigidbody2D rb2D;
        [HideInInspector] public Collider2D collectableCol;		
        public float knockBackForce = 1;

        public override void PreInit()
        {
            base.PreInit();
            
            if (!navAgent){
                navAgent = GetComponent<NavMeshAgent>();
                navAgent.updateRotation = false;
                navAgent.updateUpAxis = false;
            }
            navAgent.enabled = false;
            
            if (!collectableCol)
            {
                collectableCol = this.GetCollider((int)ELayer.Collectable);
            }
            collectableCol.enabled = false;
            interactableCol.enabled = false;
            
            if (!moveController){ moveController = GetComp<MoveController>(); }
            moveController.SPD.RegisterWithInitValue(value =>
            {
                navAgent.speed = value;
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            if (!rb2D){
                rb2D = GetComponent<Rigidbody2D>();
            }
        }
        
        public override void Init()
        {
            base.Init();
            collectableCol.enabled = true;
        }

        public void StartCollection(Transform newTarget)
        {
            target = newTarget;
            navAgent.enabled = true;
            Vector2 direction = (target.position - transform.position).normalized;
            rb2D.AddForce(direction * knockBackForce, ForceMode2D.Impulse);
            collectableCol.enabled = false;
            interactableCol.enabled = true;
        }

        private void Update()
        {
            if (navAgent.enabled && target)
            {
                navAgent.SetDestination(target.position);
            }
        }
    }
}