using System;
using System.Collections.Generic;
using CodeTao;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// Container for buffs.
    /// </summary>
    public class BuffOwner : Container<Buff>
    {
        public List<Buff> FindAll(Func<Buff, bool> condition)
        {
            List<Buff> buffs = new List<Buff>();
            foreach (Buff buff in Contents)
            {
                if (condition(buff))
                {
                    buffs.Add(buff);
                }
            }

            return buffs;
        }
    }
}