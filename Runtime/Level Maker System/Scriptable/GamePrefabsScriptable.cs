using System.Collections.Generic;
using UnityEngine;
using IA.Database.DataType;
using IA.Database.Data;

namespace IA.Database
{
    [CreateAssetMenu(fileName = "Game Item Prefab Database", menuName = "IA/Create -> Game Item Prefab Database", order = 5)]
    public class GamePrefabsScriptable : ScriptableObject
    {
        /// <summary>
        /// Prefabs List
        /// </summary>
        [SerializeField] private List<GamePrefab> gamePrefabs = new List<GamePrefab>();

        /// <summary>
        ///  Get Prefab by Enum Type
        /// </summary>
        /// <param name="itemType">Which Prefab</param>
        /// <returns></returns>
        public GameObject GetPrefab(GameItemType itemType)
        {
            return gamePrefabs.Find(x => x.ItemType == itemType).Prefab;
        }
    }
}