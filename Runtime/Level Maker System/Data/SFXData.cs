using UnityEngine;
using IA.Database.DataType;

namespace IA.Database.Data
{
    [System.Serializable]
    public class SFXData
    {
        public SFXType Type;

        public AudioClip Sound;

        [Range(0f, 1f)] public float Volume = 1f;
        [Range(-3f, 3f)] public float Pitch = 1f;
        public bool PlayOnAwake = false;
        public bool IsLoop = false;
    }
}