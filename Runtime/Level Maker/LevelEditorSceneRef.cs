#if UNITY_EDITOR
using IA.LevelMaker.Runtime;
using UnityEngine;
using IA.Utils;
using IA.Attributes;
using UnityEditor;

namespace IA.LevelMakerEditor
{
    [ExecuteAlways]
    public class LevelEditorSceneRef : MonoBehaviour
    {
        [ReadOnly(r: 0.5f, g: 0.5f, b: 0.5f, a: 0.5f)]
        public IAPrefabInstanceData InstanceData;

        [ReadOnly(r: 0.5f, g: 0.5f, b: 0.5f, a: 0.5f)]
        public string PrefabAddress;
        [ReadOnly(r: 0.5f, g: 0.5f, b: 0.5f, a: 0.5f)]
        public string PrefabBaseName;

        public delegate void SceneRefHandler(LevelEditorSceneRef _refObj);
        public event SceneRefHandler OnRemoved;
        public event SceneRefHandler OnDublicated;

        private bool isInvalid;

        public void ResetSubscriptions()
        {
            OnRemoved = null;
            OnDublicated = null;
        }

        // When the object is destroyed
        private void OnDestroy()
        {
            if (!isInvalid)
            {
                // Undo.SetCurrentGroupName("Level Item Removed");
                OnRemoved?.Invoke(this);
            }
        }

        public void Duplicate()
        {
            $"{name} Duplicated!".LogWarning(_context: gameObject);

            OnDublicated?.Invoke(this);
        }

        // Remove Button Clicked
        public void Remove()
        {
            isInvalid = true;

            $"{name} Removed!".LogWarning();

            OnRemoved?.Invoke(this);

            // DestroyImmediate(gameObject);
            Undo.DestroyObjectImmediate(gameObject);
        }

        public void UpdateInstanceData()
        {
            if (InstanceData != null)
            {
                InstanceData.Position = transform.position;
                InstanceData.Rotation = transform.rotation;
                InstanceData.LocalScale = transform.localScale;
            }
        }

        public void MakeInvalid() => isInvalid = true;
    }
}
#endif