using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
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
    
    public class UnitComponent : MonoBehaviour
    {
        public UnitController Unit { get; set; }
        
        public T GetComp<T>() where T : UnitComponent
        {
            return Unit.Link.GetComp<T>();
        }
    }
}