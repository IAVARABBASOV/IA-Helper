using System.Collections.Generic;
using UnityEngine;
using IA.Utils;


#if UNITY_EDITOR
using IA.Attributes;
#endif

namespace IA.LevelMaker.Runtime
{
    [System.Serializable]
    public class IALevelData
    {
#if UNITY_EDITOR
        [ReadOnly(r: 0.5f, g: 0.5f, b: 0.5f, a: 0.5f)]
#endif
        public string LevelName;

        public int ID;

        public IALevelData(int _id)
        {
            LevelName = GetNameTemplate(_id);
            ID = _id;
        }

        public void UpdateLevelName()
        {
            LevelName = GetNameTemplate(ID);
        }

        private string GetNameTemplate(int _id) => $"Level {_id}";

        public List<IALevelItemData> LevelItems = new List<IALevelItemData>();

        public IALevelItemData GetLevelItemByPrefabAddress(string _address)
        {
            return LevelItems.Find(x => x.ComparePrefabAddress(_address));
        }

        public bool Check(int _id) => ID.Equals(_id);
    }

    [System.Serializable]
    public class IALevelItemData
    {
        public string PrefabAddress;
        private string prefabName;

        public List<IAPrefabInstanceData> PrefabInstances = new List<IAPrefabInstanceData>();

        public string GetPrefabName()
        {
            if (prefabName.IsNullOrWhiteSpace())
                prefabName = PrefabAddress.Substring(PrefabAddress.LastIndexOf('/') + 1).Replace(".prefab", "");

            return prefabName;
        }

        public bool ComparePrefabAddress(string input) => string.Equals(PrefabAddress, input);
    }

    [System.Serializable]
    public class IAPrefabInstanceData
    {
        public string Name;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 LocalScale;
    }
}
