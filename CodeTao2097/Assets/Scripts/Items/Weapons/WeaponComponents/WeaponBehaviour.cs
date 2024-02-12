using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 执行器，基于选择器给予的位置信息执行效果。
    /// 武器拥有的多个执行器会按照顺序执行，在上一个执行器结束后才会执行下一个。
    /// </summary>
    public class WeaponBehaviour : BehaviourNode
    {
        protected List<Vector3> globalPositions;
        
        protected List<Vector3> localPositions;
        
        protected Weapon weapon;

        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            weapon = sequence.Get<Weapon>(BBKey.WEAPON);
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            globalPositions = sequence.Get<List<Vector3>>(BBKey.GLOBAL_POS);
            localPositions = sequence.Get<List<Vector3>>(BBKey.LOCAL_POS);
        }
    }
}