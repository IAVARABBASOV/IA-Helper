namespace IA.LevelSystem.Additional
{
    public interface IAdditionalDataManager
    {
        string AdditionalDataAsString { get; }
        void LoadData(string _additionalData);
        string ConvertDataToString();
        void DestroyMe();
    }
}