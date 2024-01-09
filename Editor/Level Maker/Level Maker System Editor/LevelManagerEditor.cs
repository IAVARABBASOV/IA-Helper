#if UNITY_EDITOR
namespace IA.LevelSystem.Editor
{
    //[UnityEditor.CustomEditor(typeof(BaseLevelManager))]
    public class LevelManagerEditor : UnityEditor.Editor
    {
        //public override void OnInspectorGUI()
        //{
        //    BaseLevelManager levelManager = (BaseLevelManager)target;

        //    levelManager.SetBallCount(UnityEditor.EditorGUILayout.IntField(nameof(levelManager.BallCount), levelManager.BallCount));
        //}
    }
}
#endif