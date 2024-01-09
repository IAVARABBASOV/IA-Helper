namespace IA.JsonManager.Scriptable
{
    public interface IJsonScriptable
    {
        string GetJsonString();
        void Load(string jsonString);
        void LoadDefaultData();
    }
}