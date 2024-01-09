#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using IA.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IA.Database.DataType;
using IA.Database.Data;
using IA.Database;
using IA.LevelSystem.Additional;
using IA.LevelSystem.Callback;

namespace IA.LevelSystem.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        #region Variables

        private static LevelEditorWindow levelDatabaseEditor;
        private static GameDatabase gamePropertiesDatabase;

        private GameItemType gameItemType = GameItemType.DefaultBall;
        private PropType propItemType = PropType.Box;

        private Vector2 editorInstanceGOScrollPos = new Vector2();

        private static int loadLevelIndex = 1;

        private static int activeLevelNo = 0;
        private static int orderChangeLevelNo = 0;
        private static GameObject SelectedInstanceGO;

        private bool isChangeLevelOrder;

        private static bool isBonusLevelEnvironment;
        private static bool currentEnvironment;
        private bool isEnvironmentSwitch = false;

        /// <summary>
        /// Mean that (Levels or Bonuses) Folder Name
        /// </summary>
        private static string environmentFolderName;

        /// <summary>
        /// Mean that (Level or Bonus) Environment Name
        /// </summary>
        private static string environmentName;

        #endregion

        #region Const Values

        private static string getEnvironmentPath() => $"Assets/Resources/{environmentFolderName}";

        private static string GetLevelAssetPath(int levelIndex)
        {
            return $"{getEnvironmentPath()}/{environmentName} {levelIndex}.asset";
        }

        private static string GetLevelResourcePath(int levelIndex)
        {
            return $"{environmentFolderName}/{environmentName} {levelIndex}";
        }

        private static LevelScriptable GetLevelDatabaseFromResources(int levelIndex)
        {
            return (LevelScriptable)Resources.Load(GetLevelResourcePath(levelIndex), typeof(LevelScriptable));
        }

        private static bool GetGameItemType(GameObject item, out GameItemType resultItemType)
        {
            bool thereIsEnumValue = Enum.TryParse<GameItemType>(GetItemName(item), out GameItemType gameItemType);

            if (thereIsEnumValue)
            {
                resultItemType = gameItemType;
            }
            else
            {
                resultItemType = GameItemType.DefaultBall;
            }

            return thereIsEnumValue;
        }

        private static bool GetPropItemType(GameObject item, out PropType resultItemType)
        {
            bool thereIsEnumValue = Enum.TryParse<PropType>(GetItemName(item), out PropType propItemType);

            if (thereIsEnumValue)
            {
                resultItemType = propItemType;
            }
            else
            {
                resultItemType = PropType.Box;
            }

            return thereIsEnumValue;
        }

        #endregion

        #region Builtin

        [MenuItem("IA/Open Level Maker Window")]
        public static void Open()
        {
            levelDatabaseEditor = (LevelEditorWindow)GetWindow(typeof(LevelEditorWindow), true, "Level Editor", true);

            /// Get Game Properties Database
            gamePropertiesDatabase = (GameDatabase)Resources.Load(DatabaseProperties.GameDatabaseName);

            if (gamePropertiesDatabase == null)
            {
                Debug.LogError("gamePropertiesDatabase is NULL. Please Create One!");
                levelDatabaseEditor.Close();
                return;
            }

            DefineEnvironment();

            if (isBonusLevelEnvironment)
            {
                /// Set Max Bonus Level Count
                gamePropertiesDatabase.MaxBonusLevelCount = GetMaxLevelCountFromResourceFolder(
                    match: (levelsCount) => levelsCount != gamePropertiesDatabase.MaxBonusLevelCount);
            }
            else
            {
                /// Set Max Level Count
                gamePropertiesDatabase.MaxLevelCount = GetMaxLevelCountFromResourceFolder(
                    match: (levelsCount) => levelsCount != gamePropertiesDatabase.MaxLevelCount);
            }

            /// Save Scriptable Object
            EditorUtility.SetDirty(gamePropertiesDatabase);

            /// Save Scene
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

            /// Create Scene Reference for Editor
            GameObject levelEditorReference = new GameObject("LEVEL EDITOR SCENE REFERENCES");
            levelEditorReference.AddComponent<SceneReferences>();

            Undo.ClearAll();

            /// Load first Level
            LoadLevel(loadLevelIndex);
        }

        private static void DefineEnvironment()
        {
            /// Define Environment
            if (isBonusLevelEnvironment)
            {
                environmentFolderName = "Bonus Levels";
                environmentName = "Bonus Level";
            }
            else
            {
                environmentFolderName = "Levels";
                environmentName = "Level";
            }

            levelDatabaseEditor.isEnvironmentSwitch = false;
        }

        private static int GetMaxLevelCountFromResourceFolder(System.Func<int, bool> match)
        {
            LevelScriptable[] allLevelsInFolder = Resources.LoadAll<LevelScriptable>($"{environmentFolderName}/");

            if (match(allLevelsInFolder.Length))
            {
                /// Get All Levels as Temp List
                List<LevelScriptable> levelsList = allLevelsInFolder.
                    OrderBy(x => int.Parse(x.name.Replace($"{environmentName} ", ""))).ToList();

                OrderLevelNames(levelsList);
            }

            return allLevelsInFolder.Length;
        }

        private static void OrderLevelNames(List<LevelScriptable> allLevelsInFolder)
        {
            int i = 1;

            foreach (LevelScriptable level in allLevelsInFolder)
            {
                string assetLastName = $"{getEnvironmentPath()}/{level.name}.asset";
                string assetNewName = $"Temp {i}";

                /*                Debug.LogError($"{environmentName} Last Name: {level.name}, {environmentName} new Name: {environmentName} {i}");// level.name = {environmentName} 1
                */
                AssetDatabase.RenameAsset(assetLastName, assetNewName);
                i++;
            }

            i = 1;

            foreach (LevelScriptable level in allLevelsInFolder)
            {
                string assetLastName = $"{getEnvironmentPath()}/{level.name}.asset";
                string assetNewName = $"{environmentName} {i}";

                /*                Debug.LogError($"{environmentName} Last Name: {level.name}, {environmentName} new Name: {environmentName} {i}");// level.name = {environmentName} 1
                */
                AssetDatabase.RenameAsset(assetLastName, assetNewName);
                i++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void Update()
        {
            if (Event.current.IsUndoPressed())
            {
                Undo.PerformUndo();

                Debug.Log("Undo !");
            }

            /// Check Bonus Level Check Box then Load Level or Bonus Level Environment
            if (isBonusLevelEnvironment != currentEnvironment)
            {
                currentEnvironment = isBonusLevelEnvironment;
                isEnvironmentSwitch = true;

                Close();
            }
        }

        private void OnGUI()
        {
            if (Event.current.IsDublicatePressed())
            {
                DublicateSelectedInstance();
            }

            if (Selection.activeGameObject != null)
            {
                SelectedInstanceGO = Selection.activeGameObject;
            }

            GUILayout.Label("Base Settings", EditorStyles.boldLabel);

            isBonusLevelEnvironment = EditorGUILayout.Toggle("Is Bonus Level Editor", isBonusLevelEnvironment);

            EditorGUILayout.Space(50);

            if (activeLevelNo > 0 && activeLevelNo <= gamePropertiesDatabase.MaxLevelCount && gamePropertiesDatabase.MaxLevelCount > 1)
            {
                isChangeLevelOrder = EditorGUILayout.Toggle($"Is Change {environmentName} Order", isChangeLevelOrder);

                EditorGUILayout.Space(10);

                if (isChangeLevelOrder)
                {
                    DrawChangeLevelOrder();

                    return;
                }
            }

            DrawAddToSceneButtonAndEnum();

            EditorGUILayout.Space(50);

            EditorGUILayout.BeginHorizontal();

            DrawEditorInstancesGOList();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(50);

            DrawSaveLoadButtons();
        }

        private void OnDestroy()
        {
            EditorWindowClose();
        }

        private void EditorWindowClose()
        {
            if (isEnvironmentSwitch)
            {
                /// Switch Environment
                SwitchEnvironment();

                return;
            }

            Scene activeScene = SceneManager.GetActiveScene();

            if (activeScene.isDirty)
            {
                int result = EditorUtility.DisplayDialogComplex("Close",
                "Do you want to Save Database ?", "Yes", "No", "Cancel");

                if (result == 0)
                {
                    SaveLevel(activeLevelNo);

                    Closed();

                    EditorSceneManager.SaveScene(activeScene);

                }
                else if (result == 1)
                {
                    Closed();

                    EditorSceneManager.SaveScene(activeScene);
                }
                else if (result == 2)
                {
                    OpenRoutine().StartCoroutine();
                }
            }
            else
            {
                /// Close Window
                Closed();

                EditorSceneManager.SaveScene(activeScene);
            }
        }

        private void SwitchEnvironment()
        {
            DestroyAllInstance(destroySceneReference: true);
            Undo.ClearAll();

            OpenRoutine(0.5f).StartCoroutine();
        }

        private void Closed()
        {
            DestroyAllInstance(destroySceneReference: true);

            Undo.ClearAll();

            SaveChanges();
        }

        private IEnumerator OpenRoutine()
        {
            yield return new WaitForEndOfFrame();

            levelDatabaseEditor = (LevelEditorWindow)GetWindow(typeof(LevelEditorWindow), true, "Level Editor", true);
        }

        private IEnumerator OpenRoutine(float delayInSec)
        {
            yield return new WaitForSeconds(delayInSec);

            loadLevelIndex = 1;

            Open();
        }

        #endregion

        #region Custom Methods

        private void DrawChangeLevelOrder()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField($"Change {environmentName} Order");

            EditorGUILayout.Space(-400);

            orderChangeLevelNo = Mathf.Clamp(EditorGUILayout.IntField("   ", orderChangeLevelNo, GUILayout.Width(200)), 1, gamePropertiesDatabase.MaxLevelCount);

            EditorGUILayout.EndHorizontal();

            if (orderChangeLevelNo != activeLevelNo)
            {
                EditorGUILayout.LabelField(
                $"Change the Current level and Target {environmentName} Place\n" +
                "Other levels position didn't Change", EditorStyles.boldLabel, GUILayout.Height(50));

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button($"{environmentName} {activeLevelNo} Place Change with {environmentName} {orderChangeLevelNo}"))
                {
                    int result = EditorUtility.DisplayDialogComplex($"Switch {environmentFolderName} Position",
                            $"Do You Want to Switch '{environmentName} {activeLevelNo}' to '{environmentName} {orderChangeLevelNo}' ?" +
                            $"\n\n NOTE:" +
                            $"\n'{environmentName} {activeLevelNo}' will be '{environmentName} {orderChangeLevelNo}' and" +
                            $"\n'{environmentName} {orderChangeLevelNo}' will be '{environmentName} {activeLevelNo}' !!!",
                            "Yes", "No", "");

                    if (result == 0)
                    {
                        SwitchLevelPosition(activeLevelNo, orderChangeLevelNo);

                        DestroyAllInstance(destroySceneReference: false);

                        LoadLevel(orderChangeLevelNo);
                    }

                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                /// When Order level no is active level no
                /// Select next Level no
                orderChangeLevelNo++;

                /// Active level not End Level
                if (activeLevelNo != gamePropertiesDatabase.MaxLevelCount)
                {
                    /// Check order not Greater than End Level
                    if (orderChangeLevelNo > gamePropertiesDatabase.MaxLevelCount)
                    {
                        /// Select before level
                        orderChangeLevelNo--;
                    }
                }
                else /// Active level is end level
                {
                    /// Select before level
                    orderChangeLevelNo -= 2;
                }
            }
        }

        /// SWITCH
        private void SwitchLevelPosition(int _activeLevelNo, int _otherLevelNo)
        {
            /// Get All Levels from Folder
            LevelScriptable[] allLevelsInFolder = Resources.LoadAll<LevelScriptable>($"{environmentFolderName}/");

            /// Get All Levels as Temp List
            List<LevelScriptable> levelsList = allLevelsInFolder.
                OrderBy(x => int.Parse(x.name.Replace($"{environmentName} ", ""))).
                ToList();

            /// Levels Index
            int activeLevelIndex = _activeLevelNo - 1;
            int targetIndex = _otherLevelNo - 1;

            /// Get Levels and Change them Index
            LevelScriptable activeLevel = levelsList[activeLevelIndex];
            LevelScriptable targetLevel = levelsList[targetIndex];

            levelsList.Remove(activeLevel);
            levelsList.Insert(targetIndex, activeLevel);

            levelsList.Remove(targetLevel);
            levelsList.Insert(activeLevelIndex, targetLevel);

            /// Rename All Levels in Folder
            RenameAssetsInFolder(levelsList);

            Debug.LogError($"{environmentName} {_activeLevelNo} Replace to {environmentName} {_otherLevelNo} and {environmentName} {_otherLevelNo} Replace to {environmentName} {_activeLevelNo}");
        }

        private static void RenameAssetsInFolder(List<LevelScriptable> levelsList)
        {
            int i = 1;

            /// Change Assets Name to Temporary Name, Because Current names are Exist
            foreach (LevelScriptable level in levelsList)
            {
                string assetLastName = $"{getEnvironmentPath()}/{level.name}.asset";
                string assetNewName = $"BlaBlaBla {i}";

                AssetDatabase.RenameAsset(assetLastName, assetNewName);
                i++;
            }

            /// Make Levels name Correct Name
            i = 1;
            foreach (LevelScriptable level in levelsList)
            {
                string assetLastName = $"{getEnvironmentPath()}/{level.name}.asset";
                string assetNewName = $"{environmentName} {i}";

                AssetDatabase.RenameAsset(assetLastName, assetNewName);
                i++;
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private void DrawAddToSceneButtonAndEnum()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Add Game Item:");
            /// Draw Enum
            gameItemType = (GameItemType)EditorGUILayout.EnumPopup(gameItemType);
            /// Add to Scene Button
            if (GUILayout.Button("Add to Scene"))
            {
                LevelItemData levelItemData = new LevelItemData(gameItemType)
                {
                    Position = new Vector3(0f, 0f, 0f),
                    Rotation = Quaternion.identity,
                    LocalScale = Vector3.one
                };

                /// Button Clicked, Create instance of Selected Item
                GameObject selectedItemInstance = CreateGameItemInstance(levelItemData, saveUndo: true);

                SelectInstanceGO(selectedItemInstance);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Add Prop Item:");

            propItemType = (PropType)EditorGUILayout.EnumPopup(propItemType);

            /// Add to Scene Button
            if (GUILayout.Button("Add to Scene"))
            {
                LevelPropData propData = new LevelPropData(propItemType)
                {
                    Position = new Vector3(0f, 0f, 0f),
                    Rotation = Quaternion.identity,
                    LocalScale = Vector3.one
                };

                /// Button Clicked, Create instance of Selected Item
                GameObject selectedItemInstance = CreatePropInstance(propData, saveUndo: true);

                /// Select
                SelectInstanceGO(selectedItemInstance);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawEditorInstancesGOList()
        {
            /// Define Removed GO 
            GameObject removedGO = null;
            string editorInstancGoName;

            /// Create Scroll View
            editorInstanceGOScrollPos = EditorGUILayout.BeginScrollView(editorInstanceGOScrollPos);

            SceneReferences.LoopToReferences((editorInstanceGO) =>
            {
                EditorGUILayout.BeginHorizontal();

                editorInstancGoName = GetItemName(editorInstanceGO);

                /// Select Button for Select Instance
                if (GUILayout.Toggle(SelectedInstanceGO == editorInstanceGO, $"Select: -> {editorInstancGoName}", EditorStyles.miniButton))
                {
                    SelectInstanceGO(editorInstanceGO);
                }

                /// Remove Button for Remove Instance
                if (GUILayout.Button($"X Remove: -> {editorInstancGoName}", EditorStyles.helpBox))
                {
                    removedGO = editorInstanceGO;
                }

                EditorGUILayout.EndHorizontal();
            });

            /// Scroll End
            EditorGUILayout.EndScrollView();

            /// Remove object from List and Destroy from Scene
            if (removedGO != null)
            {
                SceneReferences.RemoveItemFromList(removedGO, useUndoDestroy: true);
            }
        }

        private void DrawSaveLoadButtons()
        {
            EditorGUILayout.BeginHorizontal();

            int maxLevelCount = isBonusLevelEnvironment ? gamePropertiesDatabase.MaxBonusLevelCount : gamePropertiesDatabase.MaxLevelCount;

            EditorGUILayout.LabelField($"{environmentFolderName} Count: {maxLevelCount}");

            EditorGUILayout.LabelField($"(1 - {maxLevelCount}) -> ");
            EditorGUILayout.Space(-300);
            loadLevelIndex = Mathf.Clamp(EditorGUILayout.IntField("  ", loadLevelIndex, GUILayout.Width(5000)), 1, maxLevelCount);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button($"Create {environmentName}"))
            {
                /// Increase the Max Level Count for set next level index
                maxLevelCount++;

                if (isBonusLevelEnvironment)
                {
                    gamePropertiesDatabase.MaxBonusLevelCount = maxLevelCount;
                }
                else
                {
                    gamePropertiesDatabase.MaxLevelCount = maxLevelCount;
                }

                /// Create new Level Database Instance
                LevelScriptable new_DB = ScriptableObject.CreateInstance<LevelScriptable>();

                /// On Save Level Handler Call
                LevelEditorWindowEventCallback.InvokeSaveLevelHandler();

                /// Fill the Level Data
                FillLevelData(new_DB);

                /// Create new Scriptable Object
                AssetDatabase.CreateAsset(new_DB, GetLevelAssetPath(maxLevelCount));

                /// Save
                AssetDatabase.SaveAssets();

                /// Set as Active Level
                activeLevelNo = maxLevelCount;
                loadLevelIndex = activeLevelNo;

                EditorUtility.SetDirty(gamePropertiesDatabase);

                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

                Undo.ClearAll();

                Debug.Log($"{environmentName} {activeLevelNo} Created!");
            }

            if (GUILayout.Button($"Load {environmentName} ({loadLevelIndex})"))
            {
                LoadSelectedLevelClicked();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(25);

            if (maxLevelCount > 0)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel($"Switch {environmentName} Fast:", EditorStyles.boldLabel);

                if (activeLevelNo > 1)
                {
                    if (GUILayout.Button($"< Back to {environmentName} {activeLevelNo - 1}"))
                    {
                        loadLevelIndex = activeLevelNo - 1;
                        bool isLoaded = LoadSelectedLevelClicked();

                        if (!isLoaded)
                        {
                            loadLevelIndex++;
                        }
                    }
                }

                if (activeLevelNo < maxLevelCount)
                {
                    if (GUILayout.Button($"Next to {environmentName} {activeLevelNo + 1} >"))
                    {
                        loadLevelIndex = activeLevelNo + 1;
                        bool isLoaded = LoadSelectedLevelClicked();

                        if (!isLoaded)
                        {
                            loadLevelIndex--;
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(25);

                if (GUILayout.Button($"Save All Changes: {environmentName} {activeLevelNo}"))
                {
                    SaveLevel(activeLevelNo);

                    DestroyAllInstance();

                    Scene activeScene = SceneManager.GetActiveScene();

                    EditorSceneManager.SaveScene(activeScene);

                    LoadLevel(activeLevelNo);

                    Debug.LogError($"{environmentName} {activeLevelNo}. Saved to Database");
                }

                if (GUILayout.Button($"Show {environmentName}: {activeLevelNo} Database in Assets"))
                {
                    SelectedInstanceGO = null;

                    LevelScriptable activeLevelDatabase = GetLevelDatabaseFromResources(activeLevelNo);
                    Selection.activeObject = activeLevelDatabase;
                }
            }
        }

        private static bool LoadSelectedLevelClicked()
        {
            if (loadLevelIndex == activeLevelNo) return false;

            Scene activeScene = SceneManager.GetActiveScene();

            if (activeScene.isDirty)
            {
                var result = EditorUtility.DisplayDialogComplex("There is unsaved Data",
                $"Do you want to save {environmentName} ?", "Yes", "No", "Cancel");

                if (result == 0)
                {
                    SaveLevel(activeLevelNo);

                    levelDatabaseEditor.SaveChanges();

                    DestroyAllInstance();

                    EditorSceneManager.SaveScene(activeScene);

                    LoadSelectedLevel();

                    return true;
                }
                else if (result == 1)
                {
                    DestroyAllInstance();

                    EditorSceneManager.SaveScene(activeScene);

                    LoadSelectedLevel();

                    return true;
                }
            }
            else
            {
                LoadSelectedLevel();
            }

            return true;
        }

        private static void LoadSelectedLevel()
        {
            /// Destroy Last Instance
            DestroyAllInstance();

            /// Load Selected Level
            LoadLevel(loadLevelIndex);

            Undo.ClearAll();
        }

        private static void FillLevelData(LevelScriptable levelDatabase)
        {
            /// Create New Data
            levelDatabase.LevelData = new List<LevelItemData>();

            /// Create New Data
            levelDatabase.LevelProps = new List<LevelPropData>();

            SceneReferences.LoopToReferences((editorInstanceGO) =>
            {
                /// Get Item Name
                string itemName = GetItemName(editorInstanceGO);

                /// Convert Item Name to Item Type Enum
                bool isGameItemType = Enum.TryParse(itemName, out GameItemType gameItemType);

                bool isPropType = Enum.TryParse(itemName, out PropType propType);

                string additionalData = string.Empty;

                LevelEditorItemRef levelEditorItemRef = editorInstanceGO.GetComponent<LevelEditorItemRef>();
                IAdditionalDataManager additionalDataManager = levelEditorItemRef.GetAdditionalDataManager;
                if (additionalDataManager != null)
                {
                    additionalData = additionalDataManager.AdditionalDataAsString;
                }

                if (isGameItemType)
                {
                    /// Create Level Item Data
                    LevelItemData levelItem = new LevelItemData(gameItemType)
                    {
                        Position = editorInstanceGO.transform.position,
                        Rotation = editorInstanceGO.transform.rotation,
                        LocalScale = editorInstanceGO.transform.localScale,
                        AdditionalData = additionalData
                    };

                    /// Add data to Database
                    levelDatabase.LevelData.Add(levelItem);
                }

                if (isPropType)
                {
                    /// Create Prop Data List from Game Object
                    LevelPropData propData = new LevelPropData(propType)
                    {
                        Position = editorInstanceGO.transform.position,
                        Rotation = editorInstanceGO.transform.rotation,
                        LocalScale = editorInstanceGO.transform.localScale,
                        AdditionalData = additionalData
                    };

                    /// Add prop to Data List
                    levelDatabase.LevelProps.Add(propData);
                }
            });
        }

        private static void DestroyAllInstance(bool destroySceneReference = false)
        {
            /// Destroy References on Scene
            SceneReferences.DestroyList();

            /// Destroy References Manager when destroySceneReference: true
            if (destroySceneReference && SceneReferences.Instance != null) DestroyImmediate(SceneReferences.Instance.gameObject);
        }

        private static void LoadLevel(int levelIndex)
        {
            int maxLevelCount = isBonusLevelEnvironment ? gamePropertiesDatabase.MaxBonusLevelCount : gamePropertiesDatabase.MaxLevelCount;

            /// There is Level
            if (maxLevelCount > 0)
            {
                /// Get Database from Resource
                LevelScriptable levelDatabase = GetLevelDatabaseFromResources(levelIndex);
                if (levelDatabase)
                {
                    if (levelDatabase.LevelData.Count > 0 || levelDatabase.LevelProps.Count > 0)
                    {
                        /// Create Instance of Database
                        foreach (LevelItemData dataItem in levelDatabase.LevelData)
                        {
                            CreateGameItemInstance(dataItem);
                        }

                        /// Create Instance of Database
                        foreach (LevelPropData propData in levelDatabase.LevelProps)
                        {
                            CreatePropInstance(propData);
                        }

                        /// Set as active Level
                        activeLevelNo = levelIndex;
                    }
                    else
                    {
                        Debug.LogError($"{environmentName} Database Found but There is No Data");
                    }
                }
                else
                {
                    Debug.LogError($"{environmentName} Database Not Found on index: {levelIndex}");
                }
            }
        }

        private static GameObject CreateGameItemInstance(LevelItemData itemData, bool saveUndo = false)
        {
            /// Get Prefab via ItemType
            GameObject selectedItemPrefab = gamePropertiesDatabase.GetGamePrefabDatabase.GetPrefab(itemData.ItemType);

            GameObject instance = SceneReferences.CreateLevelItem(selectedItemPrefab,
            $"{environmentName} Editor | {itemData.ItemType}", itemData, saveUndo, out LevelEditorItemRef editorItemRef);

            return instance;
        }

        private static GameObject CreatePropInstance(LevelPropData propData, bool saveUndo = false)
        {
            ///// Get Prefab via ItemType
            //GameObject selectedItemPrefab = gamePropertiesDatabase.GetPropPrefabDatabase.GetPropPrefab(propData.PropType);

            //LevelItemData itemData = new LevelItemData(GameItemType.DefaultBall) 
            //{
            //    Position = propData.Position,
            //    Rotation = propData.Rotation,
            //    LocalScale = propData.LocalScale,
            //    AdditionalData = propData.AdditionalData
            //};

            //GameObject instance = SceneReferences.CreateLevelItem(selectedItemPrefab,
            //$"{environmentName} Editor | {propData.PropType}", itemData, saveUndo, out LevelEditorItemRef editorItemRef);

            //return instance;

            return null;
        }

        private static string GetItemName(GameObject editorInstanceGO)
        {
            /// Get Item Name
            string editorInstancGoName = editorInstanceGO.name;
            /// Define "Level Editor |" (+1 is Space)
            int startIndex = editorInstancGoName.IndexOf('|') + 1;
            /// Define substring Length
            int length = editorInstancGoName.Length - startIndex;
            /// Return Item Type as string
            editorInstancGoName = editorInstancGoName.Substring(startIndex, length);

            return editorInstancGoName;
        }

        private static void SaveLevel(int levelIndex)
        {
            /// On Save Level Handler Call
            LevelEditorWindowEventCallback.InvokeSaveLevelHandler();

            /// Get Database
            LevelScriptable levelDatabase = GetLevelDatabaseFromResources(levelIndex);

            /// Set values
            FillLevelData(levelDatabase);

            /// Save
            EditorUtility.SetDirty(levelDatabase);

            Undo.ClearAll();
        }

        private static void SelectInstanceGO(GameObject instanceGO)
        {
            /// Select
            Selection.activeObject = instanceGO;

            /// Set Current
            SelectedInstanceGO = instanceGO;
        }

        private static void DublicateSelectedInstance()
        {
            if (SelectedInstanceGO)
            {
                Selection.activeGameObject = null;

                bool isGameItem = GetGameItemType(SelectedInstanceGO, out GameItemType itemType);

                bool isPropItem = GetPropItemType(SelectedInstanceGO, out PropType propType);

                if (isGameItem)
                {
                    LevelItemData levelItemData = new LevelItemData(itemType)
                    {
                        Position = SelectedInstanceGO.transform.position + new Vector3(0, 0, 1),
                        Rotation = SelectedInstanceGO.transform.rotation,
                        LocalScale = SelectedInstanceGO.transform.localScale
                    };

                    /// Dublicate Current Instance
                    GameObject createdInstance = CreateGameItemInstance(levelItemData, saveUndo: true);

                    /// Set as Selected Instance
                    SelectInstanceGO(createdInstance);

                    Debug.Log($"{createdInstance.name} Dublicated");
                }

                if (isPropItem)
                {
                    LevelPropData propData = new LevelPropData(propType)
                    {
                        Position = SelectedInstanceGO.transform.position + new Vector3(0, 0, 1),
                        Rotation = SelectedInstanceGO.transform.rotation,
                        LocalScale = SelectedInstanceGO.transform.localScale
                    };

                    GameObject createdInstance = CreatePropInstance(propData, saveUndo: true);

                    /// Set as Selected Instance
                    SelectInstanceGO(createdInstance);

                    Debug.Log($"{createdInstance.name} Dublicated");
                }
            }
        }
        #endregion
    }
}
#endif