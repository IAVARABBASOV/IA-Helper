#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using IA.Utils;
using System.Linq;


namespace IA.AnimatorParameter
{
    [CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
    public class AnimatorParameterAttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            AnimatorParameterAttribute parameterAttribute = (AnimatorParameterAttribute)attribute;
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty animatorProperty = property.serializedObject.FindProperty(parameterAttribute.AnimatorReferenceFieldName);
            Animator animator = (Animator)animatorProperty.objectReferenceValue;

            SerializedProperty animatorTargetParameterTypeProperty = null;
            string paramTypeFieldName = parameterAttribute.ParameterTypeFieldName;
            if (!paramTypeFieldName.IsNullOrWhiteSpace())
            {
                animatorTargetParameterTypeProperty = property.serializedObject.FindProperty(paramTypeFieldName);
            }

            if (animator != null)
            {
                // Get the parameters from the Animator
                AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
                if (animatorController != null)
                {
                    AnimatorControllerParameter[] parameters = animatorController.parameters;
                    AnimatorControllerParameterType parameterType = default;

                    if (animatorTargetParameterTypeProperty != null)
                    {
                        parameterType = (AnimatorControllerParameterType)animatorTargetParameterTypeProperty.enumValueFlag;

                        parameters = parameters.Where(x => x.type == parameterType).ToArray();
                    }

                    GUIStyle popupGuiStyle = new GUIStyle(EditorStyles.popup);
                    string[] parameterNames = new string[1] { $"{parameterType} Parameter Not Found in Animator." };
                    if (parameters.Length > 0)
                    {
                        // Extract parameter names
                        parameterNames = new string[parameters.Length];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            parameterNames[i] = parameters[i].name;
                        }

                        // Display a dropdown list for the string property
                        int selectedIndex = Mathf.Max(0, System.Array.IndexOf(parameterNames, property.stringValue));
                        selectedIndex = EditorGUI.Popup(
                            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                            label.text, selectedIndex, parameterNames, popupGuiStyle);

                        // Update the string property with the selected parameter name
                        property.stringValue = parameterNames[selectedIndex];
                    }
                    else
                    {
                        popupGuiStyle.normal.textColor = Color.red;

                        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                            label.text, parameterNames[0], popupGuiStyle);
                    }

                }
            }

            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif