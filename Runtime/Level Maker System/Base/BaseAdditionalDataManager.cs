using UnityEngine;
using IA.LevelSystem.Callback;

namespace IA.LevelSystem.Additional
{
    public abstract class BaseAdditionalDataManager : MonoBehaviour, IAdditionalDataManager
    {
        [TextArea]
        [SerializeField] protected string additionalDataAsString;
        public string AdditionalDataAsString => additionalDataAsString;

        protected virtual void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                LevelEditorWindowEventCallback.OnSaveLevelHandler += LevelEditorWindowOnSaveLevelCallback;
            }
#endif
        }

        protected virtual void OnDestroy()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                LevelEditorWindowEventCallback.OnSaveLevelHandler -= LevelEditorWindowOnSaveLevelCallback;
            }
#endif        
        }

        private void LevelEditorWindowOnSaveLevelCallback() => ConvertDataToString();

        public abstract void LoadData(string _additionalData);
        public abstract string ConvertDataToString();
        public abstract void DestroyMe();
    }
}