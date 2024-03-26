using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IA.Utils
{
    /// <summary>
    /// This Script is give you ability to Get any Instance of ScriptableObject Asset
    /// But need to Addressable Package to use, keep Scriptable Asset in Addressable then you can Get access from here. 
    /// </summary>
    /// <typeparam name="T">Your ScriptableObject</typeparam>
    public abstract class SingletonScriptable<T> : ScriptableObject where T : ScriptableObject
    {
        public int Id;

        public static List<T> AssetInstances = new List<T>();

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
                SingletonScriptable<T> castedItem = item as SingletonScriptable<T>;

                /// Casted Item id is target id
                if (castedItem.Id.Equals(_id)) return item;
            }

            return default;
        }

    }
}