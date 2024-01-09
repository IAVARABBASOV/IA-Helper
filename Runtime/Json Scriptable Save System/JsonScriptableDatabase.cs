using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using IA.Attributes;
#endif

using IA.Utils;

namespace IA.JsonManager.Scriptable
{
    [CreateAssetMenu(fileName = "Json Database", menuName = "IA/Create -> Json Database", order = 2)]
    public class JsonScriptableDatabase : ScriptableObject
    {
        [SerializeField] private string FileName = "Game_Data";

        [Space]
        [SerializeField] private List<BaseJsonScriptable> jsonScriptables = new List<BaseJsonScriptable>();
        [SerializeField] private List<JsonScriptableData> jsonScriptablesData = new List<JsonScriptableData>();

        private string GetJsonString()
        {
            jsonScriptablesData.Clear();

            foreach (var item in jsonScriptables)
            {
                JsonScriptableData jsonScriptableData = new JsonScriptableData(_name: item.name)
                {
                    JsonData = item.GetJsonString()
                };

                jsonScriptablesData.Add(jsonScriptableData);
            }

            string jsonData = DataSaveLoad.ConvertToJsonText(this, (jObject) =>
            {
                jObject.Remove("jsonScriptables");

                return jObject;
            });

            return jsonData;
        }

        public void Load(string jsonString)
        {
            jsonScriptablesData.Clear();

            if (jsonString.IsNullOrWhiteSpace())
            {
                foreach (var item in jsonScriptables)
                {
                    item.LoadDefaultData();
                }

                return;
            }
            else
            {
                bool isCorrectFile = jsonString.Contains("jsonStrings");

                if (!isCorrectFile)
                {
                    foreach (var item in jsonScriptables)
                    {
                        item.LoadDefaultData();
                    }
                }
            }

            JsonUtility.FromJsonOverwrite(jsonString, this);

            int length = jsonScriptablesData.Count;

            for (int i = 0; i < length; i++)
            {
                jsonScriptables[i].Load(jsonScriptablesData[i].JsonData);
            }
        }

        internal void LoadFromJsonFile()
        {
            string jsonText = DataSaveLoad.ReadTextFromFile(FileName);
            Load(jsonText);
        }

        internal void SaveIntoJsonFile()
        {
            string jsonString = GetJsonString();

            DataSaveLoad.WriteTextIntoFile(jsonString, FileName, useEncryption: true);
        }

#if UNITY_EDITOR
        [InspectorButton("Load Game Data from Json File")]
        public void LoadGameDataJsonFile()
        {
            LoadFromJsonFile();
            "Game Data Loaded from Json File!".LogError();
        }

        [InspectorButton("Create Game Data Json File")]
        public void CreateGameDataJsonFile()
        {
            SaveIntoJsonFile();
            "Game Data Json File Created on Asset Folder!".LogError();
        }

        [InspectorButton("Delete Game Data Json File")]
        public void DeleteGameDataJsonFile()
        {
            DataSaveLoad.DeleteFile(FileName);
            "Game Data Json File Deleted from Asset Folder!".LogError();
        }
#endif
    }
}