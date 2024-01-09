using UnityEngine.Events;

namespace IA.LevelSystem.Callback
{
    public static class LevelEditorWindowEventCallback
    {
        public static event UnityAction OnSaveLevelHandler;

        public static void InvokeSaveLevelHandler() => OnSaveLevelHandler?.Invoke();
    }
}
