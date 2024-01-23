using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 组件列表，用于获取单位的组件
    /// </summary>
    public class ComponentLink
    {
        public Dictionary<Type, UnitComponent> components = new Dictionary<Type, UnitComponent>();
        
        public T GetComp<T>() where T : UnitComponent
        {
            Type type = typeof(T);
            if (components.ContainsKey(type))
            {
                return components[type] as T;
            }

            return null;
        }
        
        public void AddComponents(List<UnitComponent> unitComponents)
        {
            foreach (var unitComponent in unitComponents)
            {
                AddComponent(unitComponent);
            }
        }
        
        public void AddComponent(UnitComponent component)
        {
            Type type = component.GetType();
            if (!components.ContainsKey(type))
            {
                components.Add(type, component);
            }
            else
            {
                components[type] = component;
            }
        }
        
        public void RemoveComponent(UnitComponent component)
        {
            Type type = component.GetType();
            if (components.ContainsKey(type))
            {
                components.Remove(type);
            }
        }
    }
    
    /// <summary>
    /// 单位组件，用于获取单位其他单位组件，或单位控制器
    /// </summary>
    public class UnitComponent : MonoBehaviour
    {
        public UnitController Unit { get; set; }

        public T GetComp<T>() where T : UnitComponent
        {
            return Unit.GetComp<T>();
        }
        
        public virtual void Init(){}
        
        public virtual void Deinit(){}
    }
}