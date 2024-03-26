using UnityEngine;
using IA.Utils;

namespace IA.JsonManager.Scriptable
{
    public class JsonFileManager : Singleton<JsonFileManager>
    {
        [SerializeField] private JsonScriptableDatabase jsonDatabase = null;

        private static bool isDataSaved;

        private static bool isDataLoaded;

        #region Unity Functions

        /// <summary>
        /// Load Data On Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (!isDataLoaded)
            {
                isDataLoaded = true;
                LoadDataFromJsonFile();
            }
        }

        /// <summary>
        /// Save Data On Quit
        /// </summary>
        public void OnApplicationQuit()
        {
            if (isDataSaved) return;

            SaveGameDataToJsonFile();
        }

        /// <summary>
        /// Save Data On Pause
        /// </summary>
        /// <param name="pause"></param>
        public void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                if (isDataSaved) return;

                SaveGameDataToJsonFile();
            }
            else
            {
                isDataSaved = false;
            }
        }

        /// <summary>
        /// Save Data On App Focus Change
        /// </summary>
        /// <param name="focus"></param>
        public void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                if (isDataSaved) return;

                SaveGameDataToJsonFile();
            }
            else
            {
                isDataSaved = false;
            }
        }

        #endregion

        #region Save/Load

        private void SaveGameDataToJsonFile()
        {
            isDataSaved = true;

            jsonDatabase.SaveIntoJsonFile();
        }

        private void LoadDataFromJsonFile()
        {
            jsonDatabase.LoadFromJsonFile();
        }

        #endregion

        #region Editor Functions

#if UNITY_EDITOR

        [UnityEditor.MenuItem("IA/Json Manager/ -> Add Json File Manager")]
        public static void AddMeToScene()
        {
            string jsonFileManagerName = "IA -> Json File Manager";
            JsonFileManager jsonFileManagerGO = Object.FindFirstObjectByType<JsonFileManager>(findObjectsInactive: FindObjectsInactive.Exclude);

            if (jsonFileManagerGO == null)
            {
                GameObject go = new GameObject(jsonFileManagerName);
                JsonFileManager jsonFileManager = go.AddComponent<JsonFileManager>();

                jsonFileManager.jsonDatabase = ScriptableUtility.
                                               GetOrCreateScriptableObject<JsonScriptableDatabase>("Assets/!IA/Json Database.asset");
            }
        }

#endif

        #endregion
    }
}