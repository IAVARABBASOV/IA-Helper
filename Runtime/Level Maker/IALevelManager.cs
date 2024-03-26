using UnityEngine;
using UnityEngine.Events;
using IA.Utils;

namespace IA.LevelMaker.Runtime
{
    /// <summary>
    /// Load Levels of Level Maker
    /// </summary>
    public class IALevelManager : Singleton<IALevelManager>
    {
        [Header("Level Manager")]
        [Tooltip("Assets Label name in Addressable")]
        [SerializeField] private string levelManagerAssetsLabel = "LevelManager";

        [Tooltip("Target Level Manager Asset ID")]
        [SerializeField] private int selectedManagerID;

        [Tooltip("Target Level ID in Selected Level Manager")]
        [SerializeField] private int selectedLevelID;


        [Header("Change Level with Keyboard")]
        public bool ChangeLevelWithKeyboard_Runtime = true;


        #region Properties
        public int GetSelectedLevelID => selectedLevelID;

        #endregion

        private void Start()
        {
            IALevelManagerScriptable.LoadAllAssets(levelManagerAssetsLabel, _onLoadCompleted: LoadSelectedLevel).StartCoroutine(this);
        }

        private void Update()
        {
            ChangeLevelRuntimeWithKeyboard();
        }

        private void OnDestroy()
        {
            IALevelManagerScriptable.ReleasePrefabs();
        }

        /// <summary>
        /// Get Level Manager with ID
        /// </summary>
        /// <param name="_targetlevelManagerID">ID of Level Manager</param>
        /// <param name="e">Callback return when Manager is Ready</param>
        public void LevelManagerController(int _targetlevelManagerID, UnityAction<IALevelManagerScriptable> e)
        {
            IALevelManagerScriptable.GetInstanceRoutine(_targetlevelManagerID, levelManagerAssetsLabel, e).StartCoroutine(this, checkGameObjectIsActive: false);
        }

        public void LoadSelectedLevel()
        {
            LoadLevel(selectedManagerID, selectedLevelID,
            _outManagerID: (id) => selectedManagerID = id,
            _outLevelID: (id) => selectedLevelID = id);
        }

        public void LoadLevel(int _managerID, int _levelID, UnityAction<int> _outManagerID, UnityAction<int> _outLevelID)
        {
            // Get Max Count of Managers
            int maxLevelManagerCount = IALevelManagerScriptable.AssetInstances.Count;

            // Clamp Manager id between max count and 0
            _managerID = _managerID.RoundIndex(maxLevelManagerCount - 1);

            // Return Clamped Manager Id
            _outManagerID.Invoke(_managerID);

            // Call LevelManager Controller to Load Exactly level from Target Asset
            LevelManagerController(_managerID,
            (targetLevelManager) =>
            {
                // Clamp Level id as Manager id; then return it
                _levelID = _levelID.RoundIndex(targetLevelManager.Levels.Count - 1);
                _outLevelID.Invoke(_levelID);

                // Load Exactly Level from target Manager with its ID
                targetLevelManager.LoadSelectedLevel(_levelID, this);
            });
        }

        private void ChangeLevelRuntimeWithKeyboard()
        {
            if (!ChangeLevelWithKeyboard_Runtime) return;

            // Right Button Clicked or D clicked
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedLevelID++;

                LoadSelectedLevel();
            }

            // Left Button Clicked or A clicked
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedLevelID--;

                LoadSelectedLevel();
            }


            // Up Button Clicked or W clicked
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                ClearLastManagerPrefabInstances();

                selectedManagerID++;
                LoadSelectedLevel();
            }

            // Down Button Clicked or S clicked
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                ClearLastManagerPrefabInstances();

                selectedManagerID--;
                LoadSelectedLevel();
            }
        }

        private void ClearLastManagerPrefabInstances()
        {
            LevelManagerController(selectedManagerID,
            (targetLevelManager) =>
            {
                targetLevelManager.DestroyInstances();
            });
        }

        #region Editor Functions

#if UNITY_EDITOR

        [UnityEditor.MenuItem("IA/Level Manager/ -> Add Level Manager")]
        public static void AddMeToScene()
        {
            string jsonFileManagerName = "IA -> Level Manager";
            IALevelManager jsonFileManagerGO = Object.FindFirstObjectByType<IALevelManager>(findObjectsInactive: FindObjectsInactive.Exclude);

            if (jsonFileManagerGO == null)
            {
                GameObject go = new GameObject(jsonFileManagerName);
                go.AddComponent<IALevelManager>();
            }
        }

#endif

        #endregion
    }
}