using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomPropertyDrawer(typeof(PnShowSpriteAttribute))]
    public class ShowSpriteDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight * 2;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label);

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.objectReferenceValue != null)
                {
                    Texture2D previewTexture = AssetPreview.GetAssetPreview(property.objectReferenceValue);
                    if (previewTexture != null)
                    {
                        Rect previewRect = new Rect()
                        {
                            x = position.x + EditorGUI.IndentedRect(position).x - position.x,
                            y = position.y + EditorGUIUtility.singleLineHeight,
                            width = position.width,
                            height = EditorGUIUtility.singleLineHeight * 2
                        };
                        GUI.Label(previewRect, previewTexture);
                    }
                }
            }
            EditorGUI.EndProperty();
        }
    }
    public class PnShowSpriteAttribute : PropertyAttribute { }
#endif
}
