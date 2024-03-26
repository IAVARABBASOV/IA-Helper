#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using IA.Utils;
using System.Collections.Generic;
using IA.LevelMaker.Runtime;
using UnityEditor.AddressableAssets.Settings;
using IA.LevelMakerEditor;
using System.Linq;
using UnityEditor.SceneManagement;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEditor.PackageManager.UI;
using UnityEditor.AddressableAssets;


namespace IA.LevelMaker.Editor
{
    /// Base EditorWindow
    public partial class LevelEditorWindow : EditorWindow
    {
        private static LevelEditorWindow window;
        private static IALevelManagerScriptable lastLevelManagerScriptable = null;
        private static IALevelManagerScriptable currentLevelManagerScriptable = null;
        private const string LevelManagerScriptablePath = "Assets/!IA/Level Maker/IALevelManagerScriptable.asset";


        [MenuItem("IA/Open Level Maker Window")]
        public static void Open()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                "Addressable Assets did not Created yet, Please Open Addressables Group Window and Click to Create Addressables Settings button from:\n-> Window/Asset Management/Addressables/Groups -> Create Addressables Settings".LogWarning(Color.cyan);

                return;
            }

            ClearUndoData();
            SaveCurrentActiveScene();

            currentLevelPrefabsInstances = new List<LevelEditorSceneRef>();

            window = GetWindow<LevelEditorWindow>("Level Maker");

            if (lastLevelManagerScriptable != null)
            {
                currentLevelManagerScriptable = lastLevelManagerScriptable;
            }
            else
            {
                currentLevelManagerScriptable = ScriptableUtility.
                        GetOrCreateScriptableObject<IALevelManagerScriptable>(LevelManagerScriptablePath);

                lastLevelManagerScriptable = currentLevelManagerScriptable;
            }

            maxLevelCount = currentLevelManagerScriptable.Levels.Count;
            if (maxLevelCount > 0)
                selectedLevel = currentLevelManagerScriptable.Levels[currentLevelID];

            LoadSelectedLevelItems();

            EnableObjectsChangeEvent();

            EnableUndoListener();

            EditorApplication.delayCall += DelayCall;
        }

        private static void DelayCall()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (Event.current.Is_CTRL_S_Pressed())
            {
                ConvertLevelItemsToData();

                SaveCurrentActiveScene();
            }
        }

        private void OnGUI()
        {
            DrawHeader();

            DrawLevelChanger();

            DrawDragAndDrop();

            DrawCreateSaveLoad();
        }

        private void OnDestroy()
        {
            TryShowQAPanel(
                _onNothingToSave: Closed,
                _onNo: Closed,
                _onYes: OnCloseQAPanelYes,
                _onCancel: OnCloseQAPanelCanceled);
        }

        private void Closed()
        {
            DisableUndoListener();
            DisableObjectsChangeEvent();
            ClearCurrentHierarchyInstances();
            ClearUndoData();

            EditorApplication.delayCall -= DelayCall;
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnCloseQAPanelYes()
        {
            ConvertLevelItemsToData();
            Closed();
            SaveCurrentActiveScene();
        }

        private void OnCloseQAPanelCanceled()
        {
            OpenWindowRoutine().StartCoroutine();
        }

        private IEnumerator OpenWindowRoutine()
        {
            yield return new WaitForSecondsRealtime(0f);

            window = GetWindow<LevelEditorWindow>("Level Maker");
        }
    }

    ///  Drag Scene Object to Area, Create New Data, Duplicate Data, Remove Data Functions
    public partial class LevelEditorWindow : EditorWindow
    {
        private static List<GameObject> currentAddedPrefabs = new List<GameObject>();
        private static List<LevelEditorSceneRef> currentLevelPrefabsInstances = new List<LevelEditorSceneRef>();

        private static Vector2 scrollPos = Vector2.zero;

        private static Color yellowColor = new Color(1f, 0.7757478f, 0.1641509f, 1f);

        private static bool isDuplicateCheckInvalid;

        private static void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();

            lastLevelManagerScriptable = (IALevelManagerScriptable)EditorGUILayout.ObjectField("Target Asset:", lastLevelManagerScriptable, typeof(IALevelManagerScriptable), allowSceneObjects: false);

            if (lastLevelManagerScriptable == null) lastLevelManagerScriptable = currentLevelManagerScriptable;

            if (!lastLevelManagerScriptable.Equals(currentLevelManagerScriptable))
            {
                TryShowQAPanel(
                    _onNothingToSave: () => LevelMakerDataChanged(_convertItemsToData: false),
                    _onNo: () => LevelMakerDataChanged(_convertItemsToData: false),
                    _onYes: () => LevelMakerDataChanged(_convertItemsToData: true),
                    _onCancel: () => lastLevelManagerScriptable = currentLevelManagerScriptable);
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void LevelMakerDataChanged(bool _convertItemsToData)
        {
            currentLevelID = 0;

            if (_convertItemsToData) ConvertLevelItemsToData();
            ClearCurrentHierarchyInstances();
            SaveCurrentActiveScene();

            currentLevelManagerScriptable = lastLevelManagerScriptable;

            maxLevelCount = currentLevelManagerScriptable.Levels.Count;
            if (maxLevelCount > 0)
                selectedLevel = currentLevelManagerScriptable.Levels[currentLevelID];

            LoadSelectedLevelItems();
        }

        private static void DrawDragAndDrop()
        {
            if (maxLevelCount > 0)
            {
                if (selectedLevel != null)
                {
                    if (currentLevelPrefabsInstances.Count == 0)
                    {
                        GUILayout.Space(100);
                        GUILayout.Label("Drag & Drop to here!\nSelect Prefabs in Assets Folder!",
                        new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter });
                    }
                }

                DragObjectsToArea();
                AddPrefabsToList();
                DrawSceneReferenceList();
            }
            else
            {
                GUILayout.Space(100);
                GUILayout.Label("No Level Found!", new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter });
                GUILayout.Label("First, Create a new Level!", new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter });
            }

            currentAddedPrefabs.Clear();
        }

        // Drag Scene Object to Area
        private static void DragObjectsToArea()
        {
            Rect dropArea = new Rect(0, 0, window.maxSize.x, window.maxSize.y);

            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged is GameObject draggedGameObject && !draggedGameObject.scene.IsValid())
                            {
                                currentAddedPrefabs.Add(draggedGameObject);
                            }

                            GUI.changed = true;
                        }
                    }
                    Event.current.Use();
                    break;
            }
        }

        /// Add the targetReferenceGameObject to the sceneReferenceObjects list if it is not already present
        private static void AddPrefabsToList()
        {
            if (currentAddedPrefabs.Count > 0)
            {
                foreach (GameObject prefab in currentAddedPrefabs)
                {
                    CreateNewInstanceInScene(prefab);
                }
            }
        }

        private static AddressableAssetEntry ConvertToAddressableAsset(GameObject prefab)
        {
            AddressableAssetEntry prefabEntry;
            // Add this Prefab to Addressable Asset
            if (prefab.IsAddressableAsset())
            {
                prefabEntry = prefab.GetAddressableAssetEntry();
            }
            else
            {
                // Convert Prefab to Addressable Asset
                AddressableAssetSettings settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
                if (settings == null)
                {
                    window.Close();
                    Debug.LogError("Addressable Assets did not Created yet");

                    return null;
                }
                prefabEntry = prefab.CreateAddressableAssetEntry();
                EditorUtility.SetDirty(settings);
            }

            return prefabEntry;
        }

        private static void CreateNewInstanceInScene(GameObject prefab)
        {
            AddressableAssetEntry prefabEntry = ConvertToAddressableAsset(prefab);

            string address = prefabEntry.address;

            Undo.SetCurrentGroupName("New Data Added");
            int groupIndex = Undo.GetCurrentGroup();

            /// Create Current Prefab Instance
            GameObject prefabInstance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            prefabInstance.transform.localScale = prefab.transform.localScale;
            prefabInstance.name = GetNamingForSceneInstance(prefab.name);

            AddSceneReferenceObjectToInstance(prefabInstance, address, prefab.name);

            Undo.RegisterCreatedObjectUndo(prefabInstance, "prefabInstance Created");
            Undo.CollapseUndoOperations(groupIndex);

            EditorGUIUtility.PingObject(prefabInstance);
            Selection.activeGameObject = prefabInstance;
        }

        /*The AddSceneReferenceObjectToScene method is a private static method that is used to add a scene reference object to a scene. 
        This method takes three parameters: an IALevelItem object representing the level,
        an IAPrefabInstanceData object representing the instance data, and a GameObject representing the instance.
        At the start of the method, it adds a LevelEditorSceneRef component to the GameObject instance using the AddComponent method. 
        This LevelEditorSceneRef component is used to keep track of the instance in the scene.
        Next, it adds the LevelEditorSceneRef object to the currentLevelPrefabsInstances list. 
        This list is used to keep track of all the prefab instances in the current level.
        The method then subscribes to the OnRemoved event of the LevelEditorSceneRef object. 
        This event is triggered when the LevelEditorSceneRef object is removed from the scene. 
        When this event is triggered, it calls the OnSceneRefObjectRemovedCallback method with the LevelEditorSceneRef object and 
        the IALevelItem object as arguments. This callback method removes the LevelEditorSceneRef object from the 
        currentLevelPrefabsInstances list and the PrefabInstances list of the IALevelItem object.
        If there are no more prefab instances in the IALevelItem object, it also removes the IALevelItem object from the LevelItems 
        list of the selected level.
        Finally, the method sets the InstanceData property of the LevelEditorSceneRef object to the IAPrefabInstanceData object. 
        This property is used to store the instance data of the LevelEditorSceneRef object.*/
        private static void AddSceneReferenceObjectToInstance(GameObject _instance, string _prefabAdress, string _baseName)
        {
            LevelEditorSceneRef levelEditorSceneRef = _instance.AddComponent<LevelEditorSceneRef>();
            levelEditorSceneRef.InstanceData = new IAPrefabInstanceData();

            currentLevelPrefabsInstances.Add(levelEditorSceneRef);

            levelEditorSceneRef.OnRemoved += OnSceneRefObjectRemovedCallback;
            levelEditorSceneRef.OnDublicated += OnSceneRefObjDuplicatedCallback;

            levelEditorSceneRef.PrefabAddress = _prefabAdress;
            levelEditorSceneRef.PrefabBaseName = _baseName;

            RenameInstanceNames(_baseName);
        }

        private static void OnSceneRefObjectRemovedCallback(LevelEditorSceneRef refObj)
        {
            string _baseName = refObj.PrefabBaseName;

            // Remove Reference from List
            currentLevelPrefabsInstances.Remove(refObj);

            RenameInstanceNames(_baseName);
        }

        private static void OnSceneRefObjDuplicatedCallback(LevelEditorSceneRef item)
        {
            Undo.SetCurrentGroupName("Data Duplicated");
            int groupIndex = Undo.GetCurrentGroup();

            /// Dublicate Current Instance
            GameObject instanceDuplicate = Instantiate(item.gameObject, item.transform.position, item.transform.rotation);
            instanceDuplicate.transform.localScale = item.transform.localScale;

            LevelEditorSceneRef levelEditorSceneRef = instanceDuplicate.GetComponent<LevelEditorSceneRef>();
            currentLevelPrefabsInstances.Add(levelEditorSceneRef);

            RenameInstanceNames(levelEditorSceneRef.PrefabBaseName);

            levelEditorSceneRef.ResetSubscriptions();
            levelEditorSceneRef.OnDublicated += OnSceneRefObjDuplicatedCallback;
            levelEditorSceneRef.OnRemoved += OnSceneRefObjectRemovedCallback;

            Undo.RegisterCreatedObjectUndo(instanceDuplicate, "prefabInstance Duplicated");
            Undo.CollapseUndoOperations(groupIndex);

            EditorGUIUtility.PingObject(instanceDuplicate);
            Selection.activeGameObject = instanceDuplicate;
        }

        private static void RenameInstanceNames(string itemName)
        {
            int i = 0;
            currentLevelPrefabsInstances.Where(x => string.Equals(x.PrefabBaseName, itemName)).ToList().ForEach(x =>
            {
                string instanceName = i > 0 ? $"{itemName} ({i})" : itemName;
                x.name = GetNamingForSceneInstance(instanceName);
                x.InstanceData.Name = instanceName;

                i++;
            });
        }

        private static void DrawSceneReferenceList()
        {
            if (currentLevelPrefabsInstances.Count > 0)
            {
                LevelEditorSceneRef removedRefObj = null;
                LevelEditorSceneRef duplicatedRefObj = null;

                EditorGUILayout.Space(25);

                EditorGUILayout.PrefixLabel("Droped Game Objects:");
                EditorGUILayout.BeginVertical();
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                foreach (var item in currentLevelPrefabsInstances)
                {
                    if (item == null)
                    {
                        removedRefObj = item;
                        break;
                    }

                    EditorGUILayout.BeginHorizontal();

                    // Select target Scene Reference Object from List
                    if (GUILayout.Toggle(Selection.activeGameObject == item.gameObject
                                      || Selection.Contains(item.gameObject),
                     $"SELECT -> {item.InstanceData.Name}", "Button"))
                    {
                        if (Selection.activeGameObject != item.gameObject)
                        {
                            Undo.SetCurrentGroupName("Level Item Selected");

                            EditorGUIUtility.PingObject(item);
                            Selection.activeGameObject = item.gameObject;
                        }
                    }

                    GUILayout.Space(20);
                    Color beforeColor = GUI.backgroundColor;
                    GUI.backgroundColor = yellowColor;
                    if (GUILayout.Button($"DUPLICATE", GUILayout.Width(200)) || Event.current.Is_CTRL_D_Pressed())
                    {
                        duplicatedRefObj = item;
                    }

                    GUILayout.Space(20);

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button($"X", GUILayout.Width(30)) || Event.current.IsDeletePressed())
                    {
                        removedRefObj = item;
                    }
                    GUI.backgroundColor = beforeColor;


                    EditorGUILayout.EndHorizontal();

                    // Add a single line here
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }

                // Remove target Scene Reference Object from List
                if (removedRefObj != null) removedRefObj.Remove();

                // Dublicate target Scene Reference Object and Add new Data
                if (duplicatedRefObj != null) duplicatedRefObj.Duplicate();

                EditorGUILayout.EndScrollView();

                EditorGUILayout.EndVertical();

                GUILayout.Space(50);
            }
        }

        private static void ClearCurrentHierarchyInstances()
        {
            currentAddedPrefabs.Clear();

            if (currentLevelPrefabsInstances.Count > 0)
                currentLevelPrefabsInstances.ForEach(x =>
                {
                    if (x != null)
                    {
                        x.MakeInvalid();
                        DestroyImmediate(x.gameObject);
                    }
                });

            currentLevelPrefabsInstances.Clear();
        }

        private static void LoadSelectedLevelItems()
        {
            if (maxLevelCount > 0)
            {
                selectedLevel.LevelItems.ForEach(item =>
                {
                    // Use AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry to get the prefab entry
                    AddressableAssetEntry prefabEntry = AddressableEditorExtension.GetEntryByAddress(item.PrefabAddress);

                    if (prefabEntry == null)
                    {
                        window.Close();
                        throw new System.Exception($"Prefab Address {item.PrefabAddress} not found!");
                    }

                    // prefabEntry to get the prefab
                    GameObject prefab = (GameObject)prefabEntry.TargetAsset;

                    if (prefab != null)
                    {
                        // Load Instances
                        item.PrefabInstances.ForEach(instanceData =>
                        {
                            GameObject prefabInstance = Instantiate(prefab, instanceData.Position, instanceData.Rotation);
                            prefabInstance.transform.localScale = instanceData.LocalScale;
                            prefabInstance.name = GetNamingForSceneInstance(instanceData.Name);

                            AddSceneReferenceObjectToInstance(prefabInstance, item.PrefabAddress, item.GetPrefabName());
                        });
                    }
                });
            }
        }

        public static string GetNamingForSceneInstance(string _name) => $"Level Editor | {_name}";
    }

    /// Level Manager: Create, Save, Load, Changer
    public partial class LevelEditorWindow : EditorWindow
    {
        private static int currentLevelID;
        private static int maxLevelCount;
        private static int targetLevelID;

        private static IALevelData selectedLevel;

        private static void ConvertLevelItemsToData()
        {
            if (selectedLevel == null) return;

            selectedLevel.LevelItems.Clear();

            foreach (LevelEditorSceneRef itemRefObj in currentLevelPrefabsInstances)
            {
                itemRefObj.UpdateInstanceData();

                IALevelItemData iALevelItem = GetOrCreateLevelItemData(itemRefObj);

                iALevelItem.PrefabInstances.Add(itemRefObj.InstanceData);
            }

            currentLevelManagerScriptable.SaveAsset();
        }

        private static IALevelItemData GetOrCreateLevelItemData(LevelEditorSceneRef itemRefObj)
        {
            // Check if the data already exists
            bool isDataExist = selectedLevel.LevelItems.Exists(lvlData => lvlData.ComparePrefabAddress(itemRefObj.PrefabAddress));

            if (isDataExist)
            {
                // Return the existing data
                return selectedLevel.LevelItems.Find(lvlData => lvlData.ComparePrefabAddress(itemRefObj.PrefabAddress));
            }
            else
            {
                // Create a new data
                IALevelItemData iALevelItem = new IALevelItemData()
                {
                    PrefabAddress = itemRefObj.PrefabAddress,
                    PrefabInstances = new List<IAPrefabInstanceData>()
                };

                // Add the new data to the level items list
                selectedLevel.LevelItems.Add(iALevelItem);

                return iALevelItem;
            }
        }

        private static void DrawCreateSaveLoad()
        {
            if (maxLevelCount > 0) GUILayout.FlexibleSpace(); // Add this line to push the buttons to the bottom

            EditorGUILayout.BeginHorizontal();

            currentLevelID = maxLevelCount > 0 ? Mathf.Clamp(currentLevelID, 0, maxLevelCount - 1) : 0;

            if (GUILayout.Button("Create New Level"))
            {
                // Check if there is any unsaved data
                TryShowQAPanel(
                    _onNothingToSave: NewLevelCreatePerformed,
                    _onYes: () =>
                    {
                        ConvertLevelItemsToData();
                        NewLevelCreatePerformed();
                    },
                    _onNo: NewLevelCreatePerformed);
            }

            EditorGUILayout.EndHorizontal();

            if (maxLevelCount > 0)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

                if (GUILayout.Button($"Remove Level ({currentLevelID})"))
                {
                    TryShowQAPanel(
                            _title: "Remove Level",
                            _message: $"Do you want to Remove this Level ?\nLevel ({currentLevelID})\nAll Data of Level ({currentLevelID}) will be lost!",
                            _forceToShowDialogBox: true,
                            _disableCancelBTN: true,

                            _onYes: RemoveCurrentLevelPerformed);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

                if (GUILayout.Button($"Save All Changes"))
                {
                    ConvertLevelItemsToData();

                    ClearCurrentHierarchyInstances();

                    SaveCurrentActiveScene();

                    LoadSelectedLevelItems();
                    "All Changes are Saved!".Log();
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private static void NewLevelCreatePerformed()
        {
            ClearCurrentHierarchyInstances();

            selectedLevel = currentLevelManagerScriptable.CreateNewLevel(maxLevelCount);
            maxLevelCount = currentLevelManagerScriptable.Levels.Count;
            currentLevelID = maxLevelCount - 1;

            ClearUndoData();
            SaveCurrentActiveScene();
        }

        private static void RemoveCurrentLevelPerformed()
        {
            ClearCurrentHierarchyInstances();

            currentLevelManagerScriptable.RemoveLevel(currentLevelID);
            currentLevelManagerScriptable.SaveAsset();

            maxLevelCount = currentLevelManagerScriptable.Levels.Count;
            currentLevelID = maxLevelCount - 1;

            selectedLevel = maxLevelCount > 0 ? currentLevelManagerScriptable.Levels[currentLevelID] : null;

            ClearUndoData();
            SaveCurrentActiveScene();

            LoadSelectedLevelItems();
        }

        private static void DrawLevelChanger()
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            string maxLevelOnGUI =
                        maxLevelCount > 1 ? $"Level -> (0 - {maxLevelCount - 1})" :
                        maxLevelCount == 1 ? "Level -> (max lvl count: 1)" : "Level -> (max lvl count: 0)";

            EditorGUILayout.LabelField(maxLevelOnGUI);

            EditorGUILayout.BeginHorizontal();

            if (maxLevelCount > 1)
            {
                if (currentLevelID > 0)
                {
                    if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.PrevKey"), GUILayout.Width(30), GUILayout.ExpandWidth(false)))
                    {
                        // Check if there is any unsaved data
                        TryShowQAPanel(
                            _onNothingToSave: ChangeToPreviousPerformed,
                            _onYes: () =>
                            {
                                ConvertLevelItemsToData();
                                ChangeToPreviousPerformed();
                            },
                            _onNo: ChangeToPreviousPerformed);

                    }
                }

                GUILayout.Label($"({currentLevelID})", GUILayout.Width(30), GUILayout.ExpandWidth(false));

                if (currentLevelID < maxLevelCount - 1)
                {
                    if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.NextKey"), GUILayout.Width(30), GUILayout.ExpandWidth(false)))
                    {
                        // Check if there is any unsaved data
                        TryShowQAPanel(
                            _onNothingToSave: ChangeToNextPerformed,
                            _onYes: () =>
                            {
                                ConvertLevelItemsToData();
                                ChangeToNextPerformed();
                            },
                            _onNo: ChangeToNextPerformed);
                    }
                }

                selectedLevel = maxLevelCount > 0 ? currentLevelManagerScriptable.Levels[currentLevelID] : null;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            if (maxLevelCount > 1)
            {
                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();

                targetLevelID = EditorGUILayout.IntField("Target Level:", targetLevelID, GUILayout.Width(200));
                targetLevelID = Mathf.Clamp(targetLevelID, 0, maxLevelCount - 1);

                GUILayout.Space(10);

                if (GUILayout.Button("Go To Level", GUILayout.Width(100), GUILayout.ExpandWidth(false)))
                {
                    // Check if there is any unsaved data
                    TryShowQAPanel(
                        _onNothingToSave: () =>
                        {
                            currentLevelID = targetLevelID;
                            SelectCurrentLevelWithID();
                        },
                        _onYes: () =>
                        {
                            ConvertLevelItemsToData();
                            currentLevelID = targetLevelID;
                            SelectCurrentLevelWithID();
                        },
                        _onNo: () =>
                        {
                            currentLevelID = targetLevelID;
                            SelectCurrentLevelWithID();
                        });
                }

                GUILayout.Space(10);

                if (GUILayout.Button($"Copy From Level ({targetLevelID})", GUILayout.Width(200), GUILayout.ExpandWidth(false)))
                {
                    // Check if there is any unsaved data
                    TryShowQAPanel(
                        _title: "Copy From Level",
                        _message: $"Do you want to Copy Data\nfrom Level ({targetLevelID}) to Level ({currentLevelID}) ?\n\nAll Data of Level ({currentLevelID}) will be lost!\n\tIt will NOT Merge!!!",
                        _disableCancelBTN: true,
                        _forceToShowDialogBox: true,

                        _onYes: () =>
                        {
                            ConvertLevelItemsToData();
                            CopyFromTargetLevelPerformed();
                        });
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private static void CopyFromTargetLevelPerformed()
        {
            ClearCurrentHierarchyInstances();

            selectedLevel = currentLevelManagerScriptable.Levels[targetLevelID];
            LoadSelectedLevelItems();
            selectedLevel = currentLevelManagerScriptable.Levels[currentLevelID];

            ConvertLevelItemsToData();
            ClearCurrentHierarchyInstances();
            SaveCurrentActiveScene();

            LoadSelectedLevelItems();
        }

        private static void ChangeToPreviousPerformed()
        {
            currentLevelID = Mathf.Clamp(currentLevelID - 1, 0, maxLevelCount - 1);
            SelectCurrentLevelWithID();
        }

        private static void SelectCurrentLevelWithID()
        {
            ClearCurrentHierarchyInstances();
            SaveCurrentActiveScene();
            selectedLevel = currentLevelManagerScriptable.Levels[currentLevelID];
            LoadSelectedLevelItems();
            ClearUndoData();
        }

        private static void ChangeToNextPerformed()
        {
            currentLevelID = Mathf.Clamp(currentLevelID + 1, 0, maxLevelCount - 1);
            SelectCurrentLevelWithID();
        }

    }

    /// Hierarchical Object Change Event
    public partial class LevelEditorWindow : EditorWindow
    {
        private static void EnableObjectsChangeEvent()
        {
            ObjectChangeEvents.changesPublished += ChangesPublished;
        }

        private static void DisableObjectsChangeEvent()
        {
            ObjectChangeEvents.changesPublished -= ChangesPublished;
        }

        private static void ChangesPublished(ref ObjectChangeEventStream stream)
        {
            if (isDuplicateCheckInvalid)
            {
                isDuplicateCheckInvalid = false;
                return;
            }

            for (int i = 0; i < stream.length; ++i)
            {
                var type = stream.GetEventType(i);
                switch (type)
                {
                    // Its callback, when Duplicate Game Object with Ctrl + D or Duplicate from Convex Menu
                    case ObjectChangeKind.CreateGameObjectHierarchy:
                        {
                            string undoGroupName = Undo.GetCurrentGroupName();

                            if (undoGroupName.Contains("Paste"))
                            {
                                stream.GetCreateGameObjectHierarchyEvent(i, out var createGameObjectHierarchyEvent);

                                GameObject duplicatedInstance = EditorUtility.
                                InstanceIDToObject(createGameObjectHierarchyEvent.instanceId) as GameObject;

                                bool isEditorSceneRef = duplicatedInstance.TryGetComponent(out LevelEditorSceneRef duplicatedRefObj);

                                if (isEditorSceneRef)
                                {
                                    currentLevelPrefabsInstances.Add(duplicatedRefObj);

                                    RenameInstanceNames(duplicatedRefObj.PrefabBaseName);

                                    duplicatedRefObj.ResetSubscriptions();
                                    duplicatedRefObj.OnDublicated += OnSceneRefObjDuplicatedCallback;
                                    duplicatedRefObj.OnRemoved += OnSceneRefObjectRemovedCallback;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }

    /// Undo System
    public partial class LevelEditorWindow : EditorWindow
    {
        private static void EnableUndoListener() => Undo.undoRedoPerformed += UndoRedoPerformed;
        private static void DisableUndoListener() => Undo.undoRedoPerformed -= UndoRedoPerformed;
        private static void ClearUndoData() => Undo.ClearAll();

        private static void UndoRedoPerformed()
        {
            PerformUndoRedoDataAction();
        }

        private static void PerformUndoRedoDataAction()
        {
            isDuplicateCheckInvalid = true;

            currentAddedPrefabs.Clear();

            currentLevelPrefabsInstances = FindObjectsByType<LevelEditorSceneRef>(FindObjectsSortMode.InstanceID).ToList();

            foreach (LevelEditorSceneRef itemRefObj in currentLevelPrefabsInstances)
            {
                itemRefObj.ResetSubscriptions();
                itemRefObj.OnDublicated += OnSceneRefObjDuplicatedCallback;
                itemRefObj.OnRemoved += OnSceneRefObjectRemovedCallback;
            }
        }
    }

    public partial class LevelEditorWindow : EditorWindow
    {
        private static void SaveCurrentActiveScene()
        {
            UnityEngine.SceneManagement.Scene currentActiveScene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.SaveScene(currentActiveScene);
        }

        private static void TryShowQAPanel(string _title = null, string _message = null, UnityAction _onNothingToSave = null,
         UnityAction _onYes = null, UnityAction _onNo = null, UnityAction _onCancel = null,
         bool _forceToShowDialogBox = false, bool _disableCancelBTN = false)
        {
            UnityEngine.SceneManagement.Scene currentActiveScene = EditorSceneManager.GetActiveScene();
            bool isSceneDirty = currentActiveScene.isDirty;
            int QA_Result = -1;

            if (isSceneDirty || _forceToShowDialogBox)
            {
                bool hasTitle = !_title.IsNullOrWhiteSpace();
                bool hasMessage = !_message.IsNullOrWhiteSpace();

                QA_Result = EditorUtility.DisplayDialogComplex(
                                        title: hasTitle ? _title : "Unsaved Data",
                                        message: hasMessage ? _message : "There is Unsaved Data. Do you want to Save it ?",
                                        ok: "Yes",
                                        cancel: "No",
                                        alt: _disableCancelBTN ? "" : "Cancel");
            }

            switch (QA_Result)
            {
                case -1: _onNothingToSave?.Invoke(); break;
                case 0: _onYes?.Invoke(); break;
                case 1: _onNo?.Invoke(); break;
                case 2: _onCancel?.Invoke(); break;
            }
        }
    }
}
#endif