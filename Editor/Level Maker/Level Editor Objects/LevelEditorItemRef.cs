using IA.LevelSystem.Additional;
using System.Collections.Generic;
using UnityEngine;
using IA.Utils;

#if UNITY_EDITOR

namespace IA.LevelSystem.Editor
{
    [ExecuteInEditMode]
    public class LevelEditorItemRef : MonoBehaviour
    {
        private List<MeshData> currentMeshs = new List<MeshData>();

        [SerializeField] private Color meshColor = new Color(1f, 0f, 0f, 0.2f);

        [SerializeField] private IAdditionalDataManager additionalDataManager;

        public IAdditionalDataManager GetAdditionalDataManager
        {
            get
            {
                if (additionalDataManager == null)
                    additionalDataManager = GetComponent<IAdditionalDataManager>();

                return additionalDataManager;
            }
        }

        private void Start()
        {
            if (gameObject.CheckIsDublicated())
            {
                gameObject.RemoveSchemeName();

                /// Set Position
                transform.position = transform.position + new Vector3(0f, 0f, 1f);

                /// Add to List
                SceneReferences.AddItemToList(saveUndo: true, this.gameObject);
            }

            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

            SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (MeshFilter m_Filter in meshFilters)
            {
                currentMeshs.Add(new MeshData
                {
                    SharedMesh = m_Filter.sharedMesh,
                    SharedTransform = m_Filter.transform
                });
            }

            foreach (SkinnedMeshRenderer m_Renderers in skinnedMeshRenderers)
            {
                currentMeshs.Add(new MeshData
                {
                    SharedMesh = m_Renderers.sharedMesh,
                    SharedTransform = m_Renderers.transform
                });
            }
        }

        private void OnDestroy()
        {
            if (!SceneReferences.Instance.IsDestroyed)
            {
                SceneReferences.RemoveItemFromList(gameObject, useUndoDestroy: false);
            }
        }

        private void OnDrawGizmos()
        {
            if (currentMeshs.Count > 0)
            {
                Gizmos.color = meshColor;

                foreach (MeshData meshData in currentMeshs)
                {
                    if (meshData.SharedTransform)
                    {
                        Gizmos.DrawMesh(
                            meshData.SharedMesh, meshData.SharedTransform.position, meshData.SharedTransform.rotation, meshData.SharedTransform.lossyScale);
                    }
                    else
                    {
                        currentMeshs.Clear();
                        break;
                    }
                }
            }
        }

        [System.Serializable]
        public struct MeshData
        {
            public Mesh SharedMesh;
            public Transform SharedTransform;
        }
    }
}
#endif