using System.Collections.Generic;
using UnityEngine;
using IA.JsonManager.Scriptable;
using IA.Utils;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
using IA.Attributes;
#endif

namespace IA.LevelMaker.Runtime
{
    [CreateAssetMenu(fileName = "IA LevelManager Scriptable", menuName = "IA/Level Maker/IA Level Manager Scriptable")]
    public partial class IALevelManagerScriptable : BaseJsonScriptableSingleton<IALevelManagerScriptable>
    {
#if UNITY_EDITOR
        [ReadOnly(r: 0.5882353f, g: 0.2117647f, b: 0.8666667f, a: 0.1f)]
#endif
        [SerializeField] private List<IALevelData> levels = new List<IALevelData>();

        public List<IALevelData> Levels => levels;

        public void AddLevel(IALevelData level)
        {
            levels.Add(level);
        }

        public void RemoveLevel(IALevelData level)
        {
            levels.Remove(level);

            int id = 0;

            foreach (IALevelData _level in levels)
            {
                _level.ID = id;
                _level.UpdateLevelName();
                id++;
            }
        }

        public void RemoveLevel(int _id)
        {
            IALevelData iALevelData = levels.Find(level => level.Check(_id));

            if (iALevelData != null)
            {
                RemoveLevel(iALevelData);
            }
        }

        public IALevelData CreateNewLevel(int _id)
        {
            IALevelData iALevelData = new IALevelData(_id);

            AddLevel(iALevelData);

            return iALevelData;
        }

        public override void LoadDefaultData()
        {
            levels.Clear();
        }

        public override string GetJsonString() => DataSaveLoad.ConvertToJsonText(this, (jObject) =>
        {
            jObject.Remove(nameof(addressablePrefabLoader));
            jObject.Remove(nameof(SelectedLevelData));
            jObject.Remove(nameof(prefabInstancesCache));

            return jObject;
        });

        public override void Load(string jsonString) => JsonUtility.FromJsonOverwrite(jsonString, this);

        public IALevelData GetLevel(int _id) => levels.Find(x => x.Check(_id));

#if UNITY_EDITOR
        public void SaveAsset()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }

    public partial class IALevelManagerScriptable : BaseJsonScriptableSingleton<IALevelManagerScriptable>
    {
        private static IAAddressablePrefabLoader addressablePrefabLoader = new IAAddressablePrefabLoader();

        public IALevelData SelectedLevelData { get; private set; }
        private List<GameObject> prefabInstancesCache = new List<GameObject>();

        public List<GameObject> GetInstances => prefabInstancesCache;

        /// <summary>
        /// Release Prefab Operations from Addressable
        /// </summary>
        public static void ReleasePrefabs() => addressablePrefabLoader.ReleasePrefabs();

        // Destroy last Instances
        public void DestroyInstances() => prefabInstancesCache.DestroyList();

        public int LoadSelectedLevel(int _levelID, MonoBehaviour _dependency)
        {
            if (levels.Count == 0) return -1;

            // Destroy last Instances
            DestroyInstances();

            // Select Level
            _levelID = SelectLevelData(_levelID, out IALevelData selectedLevelData);

            SelectedLevelData = selectedLevelData;

            GenerateLevel(SelectedLevelData, _dependency);

            return _levelID;
        }

        public int SelectLevelData(int _id, out IALevelData _selectedLevelData)
        {
            // Don't exceed level id
            _id = Mathf.Clamp(_id, 0, Levels.Count - 1);

            // Get Level Data
            _selectedLevelData = GetLevel(_id);

            return _id;
        }

        public void GenerateLevel(IALevelData _selectedLevelData, MonoBehaviour _dependency)
        {
            List<string> prefabAdresses = _selectedLevelData.LevelItems.Select(x => x.PrefabAddress).ToList();

            addressablePrefabLoader.LoadAllPrefabs(prefabAdresses, OnPrefabsLoaded).StartCoroutine(_dependency, checkGameObjectIsActive: false);
        }

        private void OnPrefabsLoaded(IAAddressablePrefabLoader _prefabLoader)
        {
            foreach (KeyValuePair<string, AsyncOperationHandle<GameObject>> operationItem in _prefabLoader.OperationDictionary)
            {
                IALevelItemData iALevelItemData = SelectedLevelData.GetLevelItemByPrefabAddress(operationItem.Key);

                if (iALevelItemData != null)
                {
                    GameObject levelPrefab = operationItem.Value.Result;
                    GeneratePrefabInstances(iALevelItemData, levelPrefab);
                }
            }
        }

        public void GeneratePrefabInstances(IALevelItemData _iALevelItem, GameObject _prefab)
        {
            foreach (IAPrefabInstanceData instanceData in _iALevelItem.PrefabInstances)
            {
                GameObject prefabInstance = GameObject.Instantiate(_prefab, instanceData.Position, instanceData.Rotation);

                prefabInstance.transform.localScale = instanceData.LocalScale;

                prefabInstancesCache.Add(prefabInstance);
            }
        }
    }

    public class IAAddressablePrefabLoader
    {
        /// <summary>
        /// Prefab Address and Prefab Operation
        /// </summary>
        public Dictionary<string, AsyncOperationHandle<GameObject>> OperationDictionary { get; private set; }

        /// <summary>
        /// Load All Prefabs from Addressables with Prefab Addresses
        /// </summary>
        /// <param name="_prefabAdresses">Prefab Location in Addressable</param>
        /// <param name="_onCompleted">Data Loaded to OperationDictionary and Done!</param>
        /// <returns></returns>
        public IEnumerator LoadAllPrefabs(IList<string> _prefabAdresses, UnityAction<IAAddressablePrefabLoader> _onCompleted)
        {
            if (OperationDictionary == null)
                OperationDictionary = new Dictionary<string, AsyncOperationHandle<GameObject>>();

            yield return AddressableExtensions.LoadAllAssets<GameObject>(
                        _keys: _prefabAdresses,
                        _onCompleted: (operationDict) =>
                        {
                            OperationDictionary = operationDict;
                            _onCompleted.Invoke(this);
                        },
                        _lastOperationDictionary: OperationDictionary,
                        _mergeMode: Addressables.MergeMode.Union,
                        _releasedCachedOpOnComplete: true);
        }

        /// <summary>
        /// Release Prefab Operations from Addressable
        /// </summary>
        public void ReleasePrefabs()
        {
            if (OperationDictionary == null || OperationDictionary.Count == 0) return;

            foreach (var item in OperationDictionary)
            {
                Addressables.Release(item.Value);
            }
        }
    }

#if UNITY_EDITOR
    public class LevelManagerScriptableModificationProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions removeAssetOptions)
        {
            // Get Deleted Asset Type
            System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(path);

            // Check it as Target Asset Type
            bool isLevelManagerAsset = assetType == typeof(IALevelManagerScriptable);

            // It is Target Asset and change the ID of other ones
            if (isLevelManagerAsset)
                IALevelManagerScriptable.UpdateID().StartCoroutine();

            // Tells the internal implementation that the callback did not delete the asset. 
            // The asset will be delete by the internal implementation.
            return AssetDeleteResult.DidNotDelete;
        }
    }
#endif
}
