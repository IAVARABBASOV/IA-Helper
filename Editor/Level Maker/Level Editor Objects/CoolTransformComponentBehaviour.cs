using IA.Database.Data;
using UnityEngine;

namespace IA.LevelSystem
{
    [ExecuteAlways]
    public class CoolTransformComponentBehaviour : MonoBehaviour
    {
        public CoolTransformComponentData Data = new CoolTransformComponentData();

        public Transform ThisTransform { get; private set; }

        public int GetID => Data.ID;

        private void Awake()
        {
            ThisTransform = transform;
        }

        public void SetData(CoolTransformComponentData _data)
        {
            /// Set New Data
            Data = _data;

            SetTransformFromData();
        }

        private void Update()
        {
            /// Update Data
            if (!Application.isPlaying)
            {
                if (Data.Position != ThisTransform.position) Data.Position = ThisTransform.position;
                if (Data.Rotation != ThisTransform.rotation) Data.Rotation = ThisTransform.rotation;
                if (Data.LocalScale != ThisTransform.localScale) Data.LocalScale = ThisTransform.localScale;
            }
        }

        private void SetTransformFromData()
        {
            /// Load Data
            ThisTransform.position = Data.Position;
            ThisTransform.rotation = Data.Rotation;
            ThisTransform.localScale = Data.LocalScale;
        }
    }
}