using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using IA.Database.Data;
using IA.LevelSystem.Additional;
using IA.LevelSystem.Utils;

namespace IA.LevelSystem.Editor
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class SceneReferences : LevelMakerItemInstance
    {
        public static SceneReferences Instance { get; private set; }

        [SerializeField] private List<GameObject> instanceGOList = new List<GameObject>();

        public bool IsDestroyed;

        protected override void Awake()
        {
            base.Awake();

            Instance = this;
        }

        public void SetDestroyed()
        {
            IsDestroyed = true;
        }

        public void ResetDestroyed()
        {
            IsDestroyed = false;
        }

        public static GameObject CreateLevelItem(GameObject selectedItemPrefab, string _name, LevelItemData _data, bool saveUndo, out LevelEditorItemRef levelEditorItemRef)
        {
            /// Create Item Prefab on Scene
            GameObject selectedItemInstance = Instantiate(selectedItemPrefab, _data.Position, _data.Rotation);

            selectedItemInstance.transform.localScale = _data.LocalScale;

            levelEditorItemRef = selectedItemInstance.AddComponent<LevelEditorItemRef>();

            IAdditionalDataManager additionalDataManager = levelEditorItemRef.GetAdditionalDataManager;

            if (additionalDataManager != null)
            {
                additionalDataManager.LoadData(_data.AdditionalData);
            }

            /// Set Name
            selectedItemInstance.name = _name;

            AddItemToList(saveUndo, selectedItemInstance);

            return selectedItemInstance;
        }

        public static void AddItemToList(bool saveUndo, GameObject selectedItemInstance)
        {
            if (saveUndo) /// Check Save Undo
            {
                /// Save Item in Undo
                Undo.RegisterCreatedObjectUndo(selectedItemInstance, $"{selectedItemInstance.name} | Created");

                /// Save Current List in Undo
                Undo.RegisterCompleteObjectUndo(Instance, $"{selectedItemInstance.name} | Added to List");
            }

            /// Add item to List
            Instance.instanceGOList.Add(selectedItemInstance);
        }

        public static void RemoveItemFromList(GameObject removedGO, bool useUndoDestroy = true)
        {
            string undoRemovedName = $"{removedGO.name} | Removed from List";       /// Get Undo Name
            Undo.RegisterCompleteObjectUndo(Instance, undoRemovedName);             /// Set Undo
            Instance.instanceGOList.Remove(removedGO);                              /// Remove Item from List
            if (useUndoDestroy) Undo.DestroyObjectImmediate(removedGO);              /// Destroy Item with Undo
        }

        public static void LoopToReferences(UnityAction<GameObject> e)
        {
            foreach (GameObject item in Instance.instanceGOList)
            {
                e.Invoke(item);
            }
        }

        public static void DestroyList()
        {
            if (Instance == null) return;

            Instance.SetDestroyed();

            foreach (var item in Instance.instanceGOList)
            {
                DestroyImmediate(item);
            }

            Instance.instanceGOList.Clear();

            Instance.ResetDestroyed();
        }
    }

#endif
}