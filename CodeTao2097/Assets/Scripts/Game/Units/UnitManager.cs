using System;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class UnitManager : MonoSingleton<UnitManager>
    {
        public Transform managerPrefab;
        
        public SerializableDictionary<Type, Transform> unitManagerDict = new SerializableDictionary<Type, Transform>();
        
        public Transform GetTransform<T>() where T : UnitController
        {
            Type type = typeof(T);
            if (!unitManagerDict.ContainsKey(type))
            {
                Transform manager = Instantiate(managerPrefab, transform);
                manager.name = type.Name + "Manager";
                unitManagerDict.Add(type, manager);
            }
            return unitManagerDict[type];
        }
    }
}