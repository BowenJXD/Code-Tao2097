using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeTao
{
    public class ItemInspectorSetter : MonoBehaviour
    {
        [HorizontalGroup("rb", 0.1f)][LabelText("")]
        public bool doSetRepetitionBehavior;
        [HorizontalGroup("rb")]
        public RepetitionBehavior repetitionBehavior;
        
        [HorizontalGroup("w", 0.1f)][LabelText("")]
        public bool doSetWeight;
        [HorizontalGroup("w")]
        public float weight;
        
        [HorizontalGroup("ml", 0.1f)][LabelText("")]
        public bool doSetMaxLvl;
        [HorizontalGroup("ml")]
        public int maxLvl;
        
        [HorizontalGroup("l", 0.1f)][LabelText("")]
        public bool doSetLvl;
        [HorizontalGroup("l")]
        public int lvl;
        
        [HorizontalGroup("re", 0.1f)][LabelText("")]
        public bool doSetRelatedElements;
        [HorizontalGroup("re")]
        public List<ElementType> relatedElements = new List<ElementType>();
        
        [Button("Set")]
        void Set()
        {
            var item = GetComponent<Item>();
            if (item == null)
            {
                Debug.LogError("No Item Component Found");
                return;
            }
            if (doSetRepetitionBehavior) { item.repetitionBehavior = repetitionBehavior; }
            if (doSetWeight) { item.weight.Value = weight; }
            if (doSetMaxLvl) { item.MaxLVL.Value = maxLvl; }
            if (doSetLvl) { item.LVL.Value = lvl; }
            if (doSetRelatedElements) { item.relatedElements = relatedElements; }
        }
    }
}