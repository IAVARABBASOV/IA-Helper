using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IA.Utils
{
    public static class ScriptableUtility
    {

#if UNITY_EDITOR
        public static T GetOrCreateScriptableObject<T>(string _path = "Assets/Database.asset") where T : ScriptableObject
        {
            // Find all assets of type JsonScriptableDatabase
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");

            if (guids.Length > 0)
            {
                // Use the first found asset (you may want to add more logic based on your project requirements)
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                T database = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                return database;
            }
            else
            {
                // Check if the directory exists, create if not
                DirectoryUtility.CreateDirectoryIfNotExists(_path);

                // If not found, create a new one
                T database = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(database, _path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return database;
            }
        }
#endif
    }
}
