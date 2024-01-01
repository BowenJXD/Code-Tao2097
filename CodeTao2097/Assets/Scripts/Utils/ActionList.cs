using System;
using System.Collections.Generic;

namespace CodeTao
{
    public enum RemoveTime
    {
        OnTrigger,
        OnDeinit,
        Never,
    }
    
    public class ActionList<T>
    {
        public Dictionary<Action<T>, RemoveTime> actions = new Dictionary<Action<T>, RemoveTime>();

        public ActionList()
        {
            
        }

        public ActionList(UnitController unitController)
        {
            unitController.onDeinit += RemoveOnDeinit;
        }
        
        public void Add(Action<T> action, RemoveTime removeTime = RemoveTime.OnDeinit)
        {
            actions.Add(action, removeTime);
        }
        
        public void Invoke(T t)
        {
            List<Action<T>> removeList = new List<Action<T>>();
            foreach (var action in actions)
            {
                if (action.Key != null)
                {
                    action.Key?.Invoke(t);
                }
                else
                {
                    removeList.Add(action.Key);
                }
            }
            foreach (var action in removeList)
            {
                actions.Remove(action);
            }
            RemoveOnTrigger();
        }
        
        public void RemoveOnTrigger()
        {
            foreach (var action in actions)
            {
                if (action.Value == RemoveTime.OnTrigger)
                {
                    actions.Remove(action.Key);
                }
            }
        }
        
        public void RemoveOnDeinit()
        {
            foreach (var action in actions)
            {
                if (action.Value == RemoveTime.OnDeinit)
                {
                    actions.Remove(action.Key);
                }
            }
        }
    }
}