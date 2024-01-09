#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using IA.Utils;
using System.Collections.Generic;

namespace IA.EditorMenu
{
    public class SceneLoaderWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private Dictionary<string, bool> folderStates = new Dictionary<string, bool>();

        [MenuItem("IA/ -> Easy Scene Loader")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<SceneLoaderWindow>("Easy Scene Loader");
        }

        private string searchQuery = "";

        private void OnGUI()
        {
            GUILayout.Space(20);

            searchQuery = EditorGUILayout.TextField("Search:", searchQuery, GUILayout.Width(500));

            GUILayout.BeginVertical();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUILayout.Space(20);

            string[] guids2 = AssetDatabase.FindAssets("t:Scene");
            string currentScenePath2 = EditorSceneManager.GetActiveScene().path;

            // Display the active scene first
            GUILayout.Label("Active Scene:", EditorStyles.boldLabel);
            DisplayScene(currentScenePath2);

            // Group scenes by folder
            Dictionary<string, List<string>> sceneGroups = new Dictionary<string, List<string>>();

            GUILayout.Space(20);
            GUILayout.Label("Other Scenes:", EditorStyles.boldLabel);

            foreach (string guid in guids2)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string name = Path.GetFileNameWithoutExtension(path);
                string folder = Path.GetDirectoryName(path);

                if (path == currentScenePath2)
                {
                    // Skip the active scene as it has already been displayed
                    continue;
                }

                if (!searchQuery.IsNullOrWhiteSpace() && !name.ToLower().Contains(searchQuery.ToLower())) continue;

                if (!sceneGroups.ContainsKey(folder))
                {
                    sceneGroups[folder] = new List<string>();
                }

                sceneGroups[folder].Add(path);
            }

            // Display scenes grouped by folder
            foreach (var group in sceneGroups)
            {
                bool isFolderOpen = folderStates.ContainsKey(group.Key) ? folderStates[group.Key] : false;

                DisplayFolder(group.Key, isFolderOpen);

                if (isFolderOpen)
                {
                    foreach (var scenePath in group.Value)
                    {
                        DisplayScene(scenePath);
                    }
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DisplayFolder(string folder, bool isFolderOpen)
        {
            GUILayout.Space(10);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Space(GetFolderDepth(folder)); // Indent based on folder depth
            GUILayout.Label(isFolderOpen ? "▼" : "▶", GUILayout.Width(15)); // Collapsible folder icon
            GUILayout.Space(10);
            GUILayout.Label($"Folder: {folder}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Toggle folder visibility
            if (Event.current.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                isFolderOpen = !isFolderOpen;
                folderStates[folder] = isFolderOpen;
                Event.current.Use();
            }


            GUILayout.Space(10);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            GUILayout.Space(10);
        }

        private void DisplayScene(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);

            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            GUILayout.Label(name);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(50);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

            if (GUILayout.Button($"Open Scene: {name}", buttonStyle, GUILayout.Width(250), GUILayout.Height(23)))
            {
                LoadSceneFromPath(name, path);
                scrollPosition = Vector2.zero;
                searchQuery = string.Empty;
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Go to Path", GUILayout.Width(100)))
            {
                Object sceneAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
                Selection.activeObject = sceneAsset;
                EditorGUIUtility.PingObject(sceneAsset);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            GUILayout.Space(10);
        }

        private void LoadSceneFromPath(string sceneName, string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // Yes Save
            }
            else
            {
                return;
            }

            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            Object sceneAsset = AssetDatabase.LoadAssetAtPath<Object>(scenePath);

            ($"{sceneName} Scene Opened!").Log(Color.white, sceneAsset);
        }

        private int GetFolderDepth(string folder)
        {
            return folder.Split(Path.DirectorySeparatorChar).Length;
        }
    }
}

#endif