#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace IA.Attributes
{
    [CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
    public class SpritePreviewDrawer : PropertyDrawer
    {
        // Define the desired width and height for the preview box
        private const float PreviewWidth = 128;
        private const float PreviewHeight = 128;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw the sprite object field
            position = EditorGUI.PrefixLabel(position, label);
            EditorGUI.PropertyField(position, property, GUIContent.none);

            // Check if the property is an Object reference (a Sprite)
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                // Get the selected Sprite
                Sprite sprite = property.objectReferenceValue as Sprite;

                // Calculate the size and position for the preview box
                Rect previewRect = new Rect(position.xMax - PreviewWidth, EditorGUIUtility.singleLineHeight, PreviewWidth, PreviewHeight);

                // Draw the Sprite preview inside the box
                if (sprite != null)
                {
                    EditorGUI.DrawPreviewTexture(previewRect, sprite.texture);
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Calculate the height of the property drawer
            return EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.singleLineHeight;
        }
    }
}

#endif