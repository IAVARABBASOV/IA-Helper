using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace IA.Utils
{
    public static class AddressableExtensions
    {
        /// <summary>
        /// Load Assets from Addressable
        /// </summary>
        /// <param name="_key">Assets Key, Label or Address</param>
        /// <param name="_onCompleted">Assets Load Done</param>
        /// <param name="_lastOperationDictionary">Last Operation Dictionary</param>
        /// <param name="_mergeMode">The mode for merging the results of the found locations. 
        /// MergeMode.None - Takes the results from the first key.
        /// MergeMode.UseFirst - Takes the results from the first key.
        /// MergeMode.Union - Takes results of each key and collects items that matched any key.
        /// MergeMode.Intersection - Takes results of each key, and collects items that matched every key.</param>
        /// <param name="_releasedCachedOpOnComplete">Determine if the cached operation should be released or not</param>
        /// <typeparam name="T">Target Asset type to load from Addressable</typeparam>
        /// <returns></returns>
        public static IEnumerator LoadAllAssets<T>(string _key,
                    UnityAction<Dictionary<string, AsyncOperationHandle<T>>> _onCompleted,
                    Dictionary<string, AsyncOperationHandle<T>> _lastOperationDictionary = null,
                    Addressables.MergeMode _mergeMode = Addressables.MergeMode.None,
                    bool _releasedCachedOpOnComplete = false)
        {
            List<string> keys = new List<string> { _key };

            yield return LoadAllAssets<T>(keys, _onCompleted, _lastOperationDictionary, _mergeMode, _releasedCachedOpOnComplete);
        }

        /// <summary>
        /// Load Assets from Addressable
        /// </summary>
        /// <param name="_keys">Assets Key, Label or Address</param>
        /// <param name="_onCompleted">Assets Load Done</param>
        /// <param name="_lastOperationDictionary">Last Operation Dictionary</param>
        /// <param name="_mergeMode">The mode for merging the results of the found locations. 
        /// MergeMode.None - Takes the results from the first key.
        /// MergeMode.UseFirst - Takes the results from the first key.
        /// MergeMode.Union - Takes results of each key and collects items that matched any key.
        /// MergeMode.Intersection - Takes results of each key, and collects items that matched every key.</param>
        /// <param name="_releasedCachedOpOnComplete">Determine if the cached operation should be released or not</param>
        /// <typeparam name="T">Target Asset type to load from Addressable</typeparam>
        /// <returns></returns>
        public static IEnumerator LoadAllAssets<T>(IList<string> _keys,
                    UnityAction<Dictionary<string, AsyncOperationHandle<T>>> _onCompleted,
                    Dictionary<string, AsyncOperationHandle<T>> _lastOperationDictionary = null,
                    Addressables.MergeMode _mergeMode = Addressables.MergeMode.None,
                    bool _releasedCachedOpOnComplete = false)
        {
            Dictionary<string, AsyncOperationHandle<T>> operationDict;

            if (_lastOperationDictionary != null) operationDict = _lastOperationDictionary;
            else operationDict = new Dictionary<string, AsyncOperationHandle<T>>();

            // Load Locations of All Assets asynchronously
            AsyncOperationHandle<IList<IResourceLocation>> locations = Addressables.LoadResourceLocationsAsync(_keys, _mergeMode, typeof(T));

            yield return locations;

            // Fill all Load Operations
            List<AsyncOperationHandle> loadOperations = new List<AsyncOperationHandle>();

            foreach (IResourceLocation location in locations.Result)
            {
                AsyncOperationHandle<T> prefabLoadHandle = Addressables.LoadAssetAsync<T>(location);

                prefabLoadHandle.Completed += obj => operationDict.TryAdd(location.PrimaryKey, obj);

                loadOperations.Add(prefabLoadHandle);
            }

            // Load All Assets with Group Operation asynchronously
            yield return Addressables.ResourceManager.CreateGenericGroupOperation(loadOperations, _releasedCachedOpOnComplete);

            // Assets Load Completed
            _onCompleted.Invoke(operationDict);
        }
    }
}
