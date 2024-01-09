#if UNITY_EDITOR

using IA.LevelSystem.Additional;
using UnityEditor;
using UnityEngine;

namespace IA.LevelSystem.Editor
{
    [CustomEditor(typeof(BaseAdditionalDataManager), editorForChildClasses: true, isFallback = false)]
    public class AdditionalDataManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Convert Data to String"))
            {
                BaseAdditionalDataManager additionalDataManager = (BaseAdditionalDataManager)target;

                Undo.RecordObject(additionalDataManager, "Convert Data to String");

                additionalDataManager.ConvertDataToString();

                EditorUtility.SetDirty(additionalDataManager);
            }

            if(GUILayout.Button("Load Data"))
            {
                BaseAdditionalDataManager additionalDataManager = (BaseAdditionalDataManager)target;

                Undo.RecordObject(additionalDataManager, "Load Data");

                additionalDataManager.LoadData(additionalDataManager.AdditionalDataAsString);

                EditorUtility.SetDirty(additionalDataManager);
            }
        }
    }
}
#endif