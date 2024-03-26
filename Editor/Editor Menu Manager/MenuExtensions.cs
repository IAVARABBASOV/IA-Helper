#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using IA.Utils;

namespace IA.EditorMenu
{
    public partial class MenuExtensions : MonoBehaviour
    {
        [MenuItem("IA/Delete My Json Data and Player Prefs", priority = 0)]
        public static void DeleteAllJsonDataFiles()
        {
            int result = EditorUtility.DisplayDialogComplex("Delete Game Data Json Files", "Do you want to delete Game Data Json Files ?", "Yes", "No", "");

            if (result == 0)
            {
                DataSaveLoad.DeleteFile("Game_Data");

                PlayerPrefs.DeleteAll();

                Debug.Log("JSON FILES REMOVED from Assets!");

                AssetDatabase.Refresh();

                //IntValueEventChannel gameDataDeletedEventChannel =
                //     (IntValueEventChannel)AssetDatabase.LoadAssetAtPath("int channel path", typeof(IntValueEventChannel));

                //if (gameDataDeletedEventChannel != null)
                //{
                //    gameDataDeletedEventChannel.RaiseEvent(1);
                //}
            }
        }


        [MenuItem("GameObject/Add Seperator", false, 0)]
        public static void AddSeperatorObjectInScene()
        {
            GameObject seperator = new GameObject("-----------------------");
            seperator.SetActive(false);
        }
    }
}
#endif
