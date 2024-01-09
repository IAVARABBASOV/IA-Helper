using UnityEngine;
using IA.Utils;

namespace IA.JsonManager
{
    public class GameDataManager : Singleton<GameDataManager>
    {
        [SerializeField] private GameDataScriptable gameData = null;

        public static GameDataScriptable GameData => Instance.gameData;
    }
}
