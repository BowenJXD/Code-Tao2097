using System;
using System.Collections.Generic;
using QFramework;

namespace CodeTao
{
    public class LogStatistic : MonoSingleton<LogStatistic>
    {
        public Dictionary<string, int> eventCount = new Dictionary<string, int>();
        
        public void AddEventCount(string eventName)
        {
            if (eventCount.ContainsKey(eventName))
            {
                eventCount[eventName]++;
            }
            else
            {
                eventCount.Add(eventName, 1);
            }
        }

        private void OnDisable()
        {
            foreach (var pair in eventCount)
            {
                LogKit.I(pair.Key + ": " + pair.Value);
            }
        }
    }
}