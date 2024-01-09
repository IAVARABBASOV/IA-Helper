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
            //Debug.LogError("Load JSON Files");

            jsonDatabase.LoadFromJsonFile();
        }

        #endregion
    }
}