using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    ///  按照碰撞体重叠的单位，生成单位范围，单位数变化时next
    /// </summary>
    public class OnEnterExitCondition : ConditionBehaviour
    {
        public Collider2D baseCol;
        List<Collider2D> cols = new List<Collider2D>();
        List<UnitController> units = new List<UnitController>();
        
        public override void Init(BehaviourSequence newSequence)
        {
            base.Init(newSequence);
            if (!baseCol) baseCol = this.GetComponentInDescendants<Collider2D>();
            sequence.Set(BBKey.COLLIDER, baseCol);
            Physics2D.OverlapCollider(baseCol, new ContactFilter2D(), cols);
            baseCol.OnTriggerEnter2DEvent(OnEnter);
            baseCol.OnTriggerExit2DEvent(OnExit);
        }
        
        protected override void OnExecute()
        {
            base.OnExecute();
            UnNext();
        }

        void OnEnter(Collider2D col)
        {
            cols.Add(col);
            if (col.GetUnit())
            {
                units.Add(col.GetUnit());
            }
            Next();
        }

        void OnExit(Collider2D col)
        {
            cols.Remove(col);
            if (col.GetUnit())
            {
                units.Remove(col.GetUnit());
            }
            Next();
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            sequence.Set(BBKey.TARGETS, units);
            sequence.Set(BBKey.TARGET_COUNT, units.Count);
        }
    }
}