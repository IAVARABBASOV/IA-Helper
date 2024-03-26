using UnityEngine;

namespace IA.ScriptableEvent
{
    public static class ScriptableUtility
    {
        /// <summary>
        /// Create Scriptable Object
        /// </summary>
        /// <typeparam name="T">Type of Target Scriptable</typeparam>
        /// <param name="nameBuilder">Name of New Scriptable</param>
        /// <param name="_assetObj">Parent Asset of new Scriptable</param>
        /// <returns>New Scirptable</returns>
        public static T CreateScriptable<T>(System.Text.StringBuilder nameBuilder, Object _assetObj = null) where T : ScriptableObject
        {
            T instance = (T)ScriptableObject.CreateInstance(typeof(T));

            instance.name = nameBuilder.ToString();

#if UNITY_EDITOR
            if (_assetObj != null)
            {
                /// Create and Add Asset into Selected Scriptable Object
                UnityEditor.AssetDatabase.AddObjectToAsset(instance, _assetObj); // 
                UnityEditor.AssetDatabase.SaveAssets();
            }
#endif
            return instance;
        }

        public static void RemoveScriptableFromAsset<T>(ref T _scriptable) where T : ScriptableObject
        {
#if UNITY_EDITOR

            UnityEditor.AssetDatabase.RemoveObjectFromAsset(_scriptable);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
            _scriptable = null;
        }

        public static void RemoveScriptable<T>(ref T _scriptable) where T : ScriptableObject
        {
#if UNITY_EDITOR

            Object.DestroyImmediate(_scriptable);
#endif

            _scriptable = null;
        }
    }
}