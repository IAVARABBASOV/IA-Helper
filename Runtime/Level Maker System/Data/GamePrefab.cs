using UnityEngine;
using IA.Database.DataType;

namespace IA.Database.Data
{
    [System.Serializable]
    public class GamePrefab
    {
        public GameItemType ItemType;
        public GameObject Prefab;
    }
}