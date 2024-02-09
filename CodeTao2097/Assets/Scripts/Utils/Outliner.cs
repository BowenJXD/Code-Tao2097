using System;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeTao
{
    public class Outliner : MonoBehaviour
    {
        public float outlineWidth = 0.1f;
        public Color outlineColor;
        Material originalMaterial;
        Material outlineMaterial;
        private SpriteRenderer sp;

        private void Awake()
        {
            outlineMaterial = Global.Instance.outlineMaterial;
            sp = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            if (sp != null)
            {
                originalMaterial = sp.material;
                sp.material = outlineMaterial;
                sp.material.SetFloat("_Thickness", outlineWidth);
                sp.material.SetColor("_OutlineColor", outlineColor);
            }
            else
            {
                LogKit.I("");
            }
        }
        
        public float OutlineWidth
        {
            get => outlineWidth;
            set
            {
                outlineWidth = value;
                if (enabled)
                {
                    sp.material.SetFloat("_Thickness", outlineWidth);
                }
            }
        }
        
        public Color OutlineColor
        {
            get => outlineColor;
            set
            {
                outlineColor = value;
                if (enabled)
                {
                    sp.material.SetColor("_OutlineColor", outlineColor);
                }
            }
        }

        private void OnDisable()
        {
            if (sp && originalMaterial)
            {
                sp.material = originalMaterial;
            }
        }
    }
}