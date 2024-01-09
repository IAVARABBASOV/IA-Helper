#if UNITY_EDITOR

using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using IA.Utils;

namespace IA.Attributes
{
    [CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
    public class InspectorButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            UnityEngine.Object targetObject = target as UnityEngine.Object;
            System.Type targetType = targetObject.GetType();

            foreach (var method in targetType.GetMethods())
            {
                var buttonAttribute = (InspectorButtonAttribute)method.GetCustomAttribute(typeof(InspectorButtonAttribute), true);
                if (buttonAttribute != null)
                {
                    if (GUILayout.Button(buttonAttribute.methodName))
                    {
                        if (buttonAttribute.isQuestionBoxEnable)
                        {
                            DialogBoxParameters dialogBoxParameters = buttonAttribute.dialogBoxParameters;

                            int result = EditorUtility.DisplayDialogComplex(
                                dialogBoxParameters.Title, dialogBoxParameters.Message,
                                dialogBoxParameters.Ok, dialogBoxParameters.Cancel,
                                dialogBoxParameters.Alt);

                            if (result == 0)
                            {
                                ButtonClicked(method, buttonAttribute);
                            }
                        }
                        else
                        {
                            ButtonClicked(method, buttonAttribute);
                        }
                    }
                }
            }
        }

        private void ButtonClicked(MethodInfo method, InspectorButtonAttribute buttonAttribute)
        {
            if (buttonAttribute.isIEnumerator)
            {
                IEnumerator enumeratorMethod = (IEnumerator)method.Invoke(target, buttonAttribute.parameters);
                enumeratorMethod.StartCoroutine();
            }
            else
            {
                method.Invoke(target, buttonAttribute.parameters);
            }
        }
    }
}

#endif