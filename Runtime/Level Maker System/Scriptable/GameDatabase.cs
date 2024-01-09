using UnityEngine;

namespace IA.Database
{
    [CreateAssetMenu(fileName = "Game Database", menuName = "IA/Create -> Game Database", order = 3)]
    public class GameDatabase : ScriptableObject
    {
        public int MaxLevelCount = 0;
        public int MaxBonusLevelCount = 0;

        #region References

        [Space, SerializeField] private GamePrefabsScriptable gamePrefabDatabase = null;
        #endregion

        #region Properties

        public GamePrefabsScriptable GetGamePrefabDatabase => gamePrefabDatabase;
        #endregion
    }
}