using UnityEngine;

namespace IA.Database.Data
{
    [System.Serializable]
    public abstract class BaseData : IData
    {
        [SerializeField] private int id;
        public int ID => id;

        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 LocalScale;
    }

    public interface IData
    {
        int ID { get; }
    }
}