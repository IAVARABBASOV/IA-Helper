using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using IA.Attributes;
using UnityEditor;
#endif

using IA.Utils;

namespace IA.JsonManager.Scriptable
{
    /* This code defines the JsonScriptableDatabase class, which is a ScriptableObject used for managing and saving JSON data.
     It includes methods for loading and saving data from/to a JSON file, as well as loading default data and deleting the JSON file.
     The Load method deserializes the JSON string and updates the scriptable object's properties accordingly.
     The SaveIntoJsonFile method converts the scriptable object's data into a JSON string and saves it into a file.
     The LoadFromJsonFile method reads the JSON file and calls the Load method to update the scriptable object's data.
     The CreateGameDataJsonFile method calls the SaveIntoJsonFile method to create a JSON file with the scriptable object's data.
     The DeleteGameDataJsonFile method deletes the JSON file.
     The GetJsonString method converts the scriptable object's data into a JSON string.
     The jsonScriptables and jsonScriptablesData lists store the scriptable objects and their corresponding data.
     The FileName field specifies the name of the JSON file.
     The [CreateAssetMenu] attribute allows creating instances of this class as ScriptableObjects from the Unity editor.
     The [InspectorButton] attributes create buttons in the Unity editor for loading, creating, and deleting the JSON file.
     The internal access modifier restricts the usage of certain methods to within the assembly.
     The #if UNITY_EDITOR directive includes code that is only compiled in the Unity editor.
     The namespace IA.JsonManager.Scriptable groups related classes together.
     The IA.Utils namespace contains utility classes and methods. */
    [CreateAssetMenu(fileName = "Json Database", menuName = "IA/Create -> Json Database", order = 2)]
    public class JsonScriptableDatabase : ScriptableObject
    {
        [SerializeField] private string FileName = "Game_Data";

        [Space]
        [SerializeField, SerializeReference] private List<BaseJsonScriptable> jsonScriptables = new List<BaseJsonScriptable>();
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
            "Game Data Loaded from Json File!".Log(Color.white);
        }

        [InspectorButton("Create Game Data Json File")]
        public void CreateGameDataJsonFile()
        {
            SaveIntoJsonFile();
            "Game Data Json File Created on Asset Folder!".Log(Color.cyan);
        }

        [InspectorButton("Delete Game Data Json File")]
        public void DeleteGameDataJsonFile()
        {
            DataSaveLoad.DeleteFile(FileName);
            "Game Data Json File Deleted from Asset Folder!".Log(Color.yellow);
        }
#endif
    }
}