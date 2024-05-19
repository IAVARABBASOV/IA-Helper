using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.Utils;
using UnityEngine.Events;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
using IA.Attributes;
#endif

namespace IA.JsonManager.Scriptable
{
    /// <summary>
    /// This Script is give you ability to Get any Instance of ScriptableObject Asset
    /// But need to Addressable Package to use, keep Scriptable Asset in Addressable then you can Get access from here. 
    /// </summary>
    /// <typeparam name="T">Your ScriptableObject</typeparam>
    public abstract class BaseJsonScriptableSingleton<T> : BaseJsonScriptable where T : ScriptableObject
    {
        #if UNITY_EDITOR
        [ReadOnly] 
        #endif
        public int Id = -1;

        public static List<T> AssetInstances = new List<T>();

        private void Awake()
        {
            UpdateID().StartCoroutine();
        }

        /// <summary>
        /// Load and Get Target Asset from Addressable
        /// 1 Assets Load and Keeping in cache during Play Time
        /// Then you can Get desired Asset.
        /// </summary>
        /// <param name="_id">Defined ID of Asset</param>
        /// <param name="_addressLabel">Addressable Label</param>
        /// <param name="e">Target Asset will return when ready</param>
        /// <returns>Coroutine call Require as a dependency</returns> 
        /// <summary>
        public static IEnumerator GetInstanceRoutine(int _id, string _addressLabel, UnityAction<T> e)
        {
            if (AssetInstances.Count > 0)
            {
                InvokeTargetAsset(_id, e);

                yield break;
            }
            else yield return LoadAllAssets(_addressLabel, _onLoadCompleted: () => InvokeTargetAsset(_id, e));

            yield break;
        }

        public static IEnumerator LoadAllAssets(string _addressLabel, UnityAction _onLoadCompleted)
        {
            yield return AddressableExtensions.LoadAllAssets<T>(
            _key: _addressLabel,
            _onCompleted: (opDict) =>
            {
                AssetInstances.Clear();

                foreach (var item in opDict)
                {
                    AssetInstances.Add(item.Value.Result);
                }

                _onLoadCompleted.Invoke();
            });
        }

        public static void InvokeTargetAsset(int _id, UnityAction<T> e)
        {
            T foundAsset = GetTargetAsset(_id);

            if (foundAsset) e.Invoke(foundAsset);
        }

        public static T GetTargetAsset(int _id)
        {
            foreach (T item in AssetInstances)
            {
                // Check Item with ID, first convert it to Base for able to check
                BaseJsonScriptableSingleton<T> castedItem = item as BaseJsonScriptableSingleton<T>;

                /// Casted Item id is target id
                if (castedItem.Id.Equals(_id)) return item;
            }

            return default;
        }

        public override void LoadDefaultData() { }

#if UNITY_EDITOR
        public static IEnumerator UpdateID()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            List<BaseJsonScriptableSingleton<T>> assets = new List<BaseJsonScriptableSingleton<T>>();

            // Get Assets GUIDs
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            // Fill Assets List
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    BaseJsonScriptableSingleton<T> castedAsset = asset as BaseJsonScriptableSingleton<T>;

                    assets.Add(castedAsset);
                }
            }

            // Order Assets by ID, moving items with ID -1 to the end of the list
            assets = assets.OrderBy(x => x.Id == -1 ? int.MaxValue : x.Id).ToList();

            int newID = 0;

            // Update IDs
            foreach (var asset in assets)
            {
                asset.Id = newID;

                EditorUtility.SetDirty(asset);

                newID++;
            }

            AssetDatabase.SaveAssets();

            yield break;
        }

#endif
    }
}
