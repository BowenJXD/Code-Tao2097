﻿using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 单位控制器，用于管理单位的生命周期
    /// </summary>
    public abstract class UnitController : MonoBehaviour
    {
        public ComponentLink Link { get; private set; }

        public T GetComp<T>() where T : UnitComponent
        {
            return Link?.GetComp<T>();
        }

        /// <summary>
        /// Will be called from global when scene is loaded,
        /// Used to bind components
        /// </summary>
        public virtual void OnSceneLoaded()
        {
            SetUp();
        }

        /// <summary>
        /// Will be called once when the gameObject is created or when the scene is loaded
        /// </summary>
        public virtual void SetUp()
        {
            if (Link == null){
                Link = new ComponentLink();
                List<UnitComponent> unitComponents = GetComponents<UnitComponent>().ToList(); 
                unitComponents.AddRange(this.GetComponentsInDescendants<UnitComponent>());
                foreach (var unitComponent in unitComponents)
                {
                    unitComponent.Unit = this;
                    Link.AddComponent(unitComponent);
                }
            }
            
            foreach (var component in Link.components.Values)
            {
                component.SetUp();
            }

            if (!ColliderManager.Instance.CheckRegistered(this)){
                List<Collider2D> colliders = this.GetComponentsInDescendants<Collider2D>();
                foreach (var col in colliders)
                {
                    ColliderManager.Instance.Register(this, col);
                }
            }
        }
        
        public Action onInit;
        
        /// <summary>
        /// Set gameObject to be active, need to be called manually
        /// </summary>
        public virtual void Init()
        {
            foreach (var component in Link.components.Values)
            {
                component.Init();
            }
            onInit?.Invoke();
            onInit = null;
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Used by UnitPool to return gameObject to pool
        /// </summary>
        public Action onDeinit;
        
        /// <summary>
        /// Used to deactivate gameObject
        /// </summary>
        public virtual void Deinit()
        {
            onDeinit?.Invoke();
            onDeinit = null;
            foreach (var component in Link.components.Values)
            {
                component.Deinit();
            }
        }
    }
}