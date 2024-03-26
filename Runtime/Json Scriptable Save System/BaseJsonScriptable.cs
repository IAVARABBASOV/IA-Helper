using UnityEngine;
using IA.Utils;

namespace IA.JsonManager.Scriptable
{
    public abstract class BaseJsonScriptable : ScriptableObject
    {
        public virtual string GetJsonString() => DataSaveLoad.ConvertToJsonText(this);

        public virtual void Load(string jsonString) => JsonUtility.FromJsonOverwrite(jsonString, this);

        public abstract void LoadDefaultData();
    }
}