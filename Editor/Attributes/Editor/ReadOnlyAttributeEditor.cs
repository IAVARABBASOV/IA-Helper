#if UNITY_EDITOR

using IA.Utils;
using UnityEditor;
using UnityEngine;

namespace IA.Attributes
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginDisabledGroup(true);

            ReadOnlyAttribute readOnlyAttribute = (ReadOnlyAttribute)attribute;

            if (readOnlyAttribute != null)
            {
                if (!readOnlyAttribute.InspectorColor.IsNull())
                {
                    Color color = readOnlyAttribute.InspectorColor;

                    Rect backgroundRect = new Rect(position.x, position.y, position.width, position.height);
                    EditorGUI.DrawRect(backgroundRect, color); // Set your desired background color
                }
            }

            EditorGUI.PropertyField(position, property, label, true);

            EditorGUI.EndDisabledGroup();

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Calculate the height of the property drawer
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}
#endif