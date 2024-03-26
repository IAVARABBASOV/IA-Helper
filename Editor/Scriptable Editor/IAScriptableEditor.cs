#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using IA.Attributes;
using System.Collections;
using System.Reflection;
using IA.Utils;

namespace IA.Scriptable.Editor
{
    public class IAScriptableEditor : UnityEditor.Editor
    {
        protected SerializedProperty targetObjProperties;
        private string[] subClassNames;
        private string itemName;

        public string TargetClassAssembly { get; protected set; }
        public string TargetNamespaceToClassNameStart { get; protected set; }
        public System.Type TargetTypeForCreate { get; protected set; }
        public string TargetPropertyName { get; protected set; }

        public string AddItemLabelName { get; protected set; }
        public float AddItemLabelWidth { get; protected set; } 

        public string AddButtonName { get; protected set; }
        public float AddButtonWidth { get; protected set; }

        public string RemoveButtonName { get; protected set; }
        public float RemoveButtonWidth { get; protected set; }

        protected virtual void OnEnable()
        {
            /// Get Target Property from Target class
            targetObjProperties = serializedObject.FindProperty(TargetPropertyName);
            FetchRewardTypeNames();
        }

        private void FetchRewardTypeNames()
        {
            // Fetch all types that implement the IReward interface
            System.Type[] subClassTypes = IAScriptableEditorUtility.GetSubclasTypes(TargetTypeForCreate);

            subClassNames = IAScriptableEditorUtility.GetSubclassNames(subClassTypes);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display a button to add a reward to the list
            GUILayout.Space(10);

            DrawAddButton();

            GUILayout.Space(10);

            DrawList();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAddButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(AddItemLabelName, GUILayout.Width(AddItemLabelWidth));
            itemName = GUILayout.TextField(itemName);

            GUILayout.EndHorizontal();

            if (GUILayout.Button(AddButtonName, GUILayout.Width(AddButtonWidth)))
            {
                GenericMenu menu = IAScriptableEditorUtility.GetGenericMenu(subClassNames, (_classFullName, menu) => 
                {
                    string className = IAScriptableEditorUtility.GetClassNameFromFullName(_classFullName, TargetNamespaceToClassNameStart);

                    if (className != nameof(TargetTypeForCreate))
                    {
                        menu.AddItem(new GUIContent(className), false, () => AddItem(_classFullName));
                    }
                });

                menu.ShowAsContext();
            }
        }

        protected virtual void DrawList()
        {
            Object targetObjForRemove = null;

            EditorGUI.indentLevel++;
            for (int i = 0; i < targetObjProperties.arraySize; i++)
            {
                SerializedProperty arrayElementProperty = targetObjProperties.GetArrayElementAtIndex(i);
                UnityEngine.Object targetObj = arrayElementProperty.objectReferenceValue;

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.ObjectField(targetObj, TargetTypeForCreate, false);

                /// Show Remove Button nearby Item
                if (GUILayout.Button(RemoveButtonName, GUILayout.Width(RemoveButtonWidth)))
                {
                    targetObjForRemove = targetObj;

                    targetObjProperties.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();

                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;

            /// Remove Asset From Folder
            if (targetObjForRemove != null)
            {
                AssetDatabase.RemoveObjectFromAsset(targetObjForRemove);
                AssetDatabase.SaveAssets();
            }

            DrawInspectorButtons();
        }

        private void DrawInspectorButtons()
        {
            System.Reflection.MethodInfo[] methods = target.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

            foreach (System.Reflection.MethodInfo method in methods)
            {
                InspectorButtonAttribute[] attributes = (InspectorButtonAttribute[])method.GetCustomAttributes(typeof(InspectorButtonAttribute), true);

                if (attributes.Length > 0)
                {
                    InspectorButtonAttribute inspectorButtonAttribute = attributes[0];

                    if (GUILayout.Button(inspectorButtonAttribute.methodName))
                    {
                        if (inspectorButtonAttribute.isQuestionBoxEnable)
                        {
                            DialogBoxParameters dialogBoxParameters = inspectorButtonAttribute.dialogBoxParameters;

                            int result = EditorUtility.DisplayDialogComplex(
                                dialogBoxParameters.Title, dialogBoxParameters.Message,
                                dialogBoxParameters.Ok, dialogBoxParameters.Cancel,
                                dialogBoxParameters.Alt);

                            if (result == 0)
                            {
                                AttributButtonClicked(method, inspectorButtonAttribute);
                            }
                        }
                        else
                        {
                            AttributButtonClicked(method, inspectorButtonAttribute);
                        }
                    }
                }
            }
        }

        private void AttributButtonClicked(MethodInfo method, InspectorButtonAttribute inspectorButtonAttribute)
        {
            if (inspectorButtonAttribute.isIEnumerator)
            {
                IEnumerator enumeratorMethod = (IEnumerator)method.Invoke(target, inspectorButtonAttribute.parameters);
                enumeratorMethod.StartCoroutine();
            }
            else
            {
                method.Invoke(target, inspectorButtonAttribute.parameters);
            }
        }

        private void AddItem(string _classfullName)
        {
            /// Define fullname for Selected class
            System.Text.StringBuilder typeNameBuilder = new System.Text.StringBuilder(_classfullName);
            typeNameBuilder.Append($", {TargetClassAssembly}");

            /// Get Selected class type
            System.Type ty = System.Type.GetType(typeNameBuilder.ToString());

            /// Create SelectedScriptable Class
            ScriptableObject selectedScriptableClass = ScriptableObject.CreateInstance(ty);

            /// Get Class Name and Set to Loot
            string className = itemName;

            /// If user didn't give name show class name
            if (string.IsNullOrEmpty(itemName) || string.IsNullOrWhiteSpace(itemName))
            {
                className = IAScriptableEditorUtility.GetClassNameFromFullName(_classfullName, TargetNamespaceToClassNameStart);
            }

            /// Set nameof Class
            selectedScriptableClass.name = className;

            /// Create and Add Asset into Selected Scriptable Object
            AssetDatabase.AddObjectToAsset(selectedScriptableClass, target); // 
            AssetDatabase.SaveAssets();

            /// Add New Asset to List
            targetObjProperties.arraySize++;
            targetObjProperties.GetArrayElementAtIndex(targetObjProperties.arraySize - 1).objectReferenceValue = selectedScriptableClass;

            /// Apply all things
            serializedObject.ApplyModifiedProperties();

            itemName = "";
        }
    }
}

#endif