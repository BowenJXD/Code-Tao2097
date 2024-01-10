using System;
using CodeTao;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace CodeTao
{
    /// <summary>
    /// 悬浮文字管理器
    /// </summary>
    public class FloatingTextManager : MonoSingleton<FloatingTextManager>
    {
        public float faceBrightness;
        public float outlineDarkness;
        public float minTextSize;
        public float maxTextSize;
        public float damageSizeIncrement;
        public float textDuration;
        
        public TextMeshProUGUI textPrefab;
        public ObjectPool<TextMeshProUGUI> floatingTextPool;

        // Start is called before the first frame update
        void Start()
        {
            floatingTextPool = new ObjectPool<TextMeshProUGUI>(() =>
                {
                    TextMeshProUGUI instance = Instantiate(textPrefab);
                    return instance;
                }, prefab =>
                {
                    prefab.gameObject.SetActive(true);
                }
                , prefab =>
                {
                    prefab.gameObject.SetActive(false);
                }
                , prefab => { Destroy(prefab); }
                , true, 10);
            DamageManager.Instance.damageAfter += GenerateDamageText;
        }

        public void GenerateDamageText(Damage damage)
        {
            TextMeshProUGUI newText = floatingTextPool.Get();
            
            float DMG = damage.GetDamageValue();
            Color color = damage.DamageElement.GetColor();
            Vector2 position = damage.Target.transform.position;
            
            newText.text = Math.Round(DMG, 2).ToString();
            newText.fontSize = Mathf.Clamp(minTextSize + DMG / damageSizeIncrement, minTextSize, maxTextSize);
            newText.transform.position = position;
            newText.transform.SetParent(transform);
        
            newText.faceColor = color * faceBrightness;
            Color outlineColour = color / outlineDarkness;
            outlineColour.a = color.a;
            newText.outlineColor = outlineColour;
            newText.Show();

            ActionKit.Delay(textDuration, () =>
            {
                if (!newText) return;
                floatingTextPool.Release(newText);
            }).Start(this);
        }

        public void SetText(string text, Color color)
        {
            textPrefab.text = text;
            textPrefab.faceColor = color * faceBrightness;
            Color outlineColour = color / outlineDarkness;
            outlineColour.a = color.a;
            textPrefab.outlineColor = outlineColour;
        }
    }
}