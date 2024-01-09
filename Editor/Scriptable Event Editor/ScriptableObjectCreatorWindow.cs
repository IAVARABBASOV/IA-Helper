#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Linq;
using IA.Utils;
using System.Text;
using IA.ScriptableEvent.Channel;

public class ScriptableObjectCreatorWindow : EditorWindow
{
    private string searchQuery = "";
    private string previousSearchQuery = "";
    private double searchQueryLastChangeTime;
    private const double searchQueryChangeDelay = 0.5; // Adjust the delay time as needed
    private int searchQueryChangeCounter = 0;
    private bool canSearch = false;
    private static int maxSearchLength = 10;


    private System.Type[] filteredTypes;
    private System.Type selectedType;
    private string overridedSelectedTypeName = "";
    private string selectedTypeDefaultValue = "default";
    private int channelOrderInDropMenu = 0;

    private Vector2 scrollPosition;


    [MenuItem("IA/Channel && Listener Class Generator")]
    public static void ShowWindow()
    {
        GetWindow<ScriptableObjectCreatorWindow>("Channel && Listener Class Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Search Properties", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Max Count of Found Types:");
        EditorGUILayout.Space(-2000);
        maxSearchLength = EditorGUILayout.IntField("   ", maxSearchLength, GUILayout.Width(650));

        GUILayout.EndHorizontal();

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5)); // Horizontal line

        GUILayout.Space(20);

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5)); // Horizontal line
        // Search bar
        GUILayout.Label("Search Class/Struct Type for Generate Channel && Listener Class", EditorStyles.boldLabel);
        searchQuery = EditorGUILayout.TextField("Search:", searchQuery);

        // Check if the text has stopped changing after a short delay
        if (searchQuery != previousSearchQuery && searchQueryChangeCounter == 0)
        {
            searchQueryLastChangeTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += OnSearchQueryChanged;
            searchQueryChangeCounter++;

            if (!searchQuery.IsNullOrWhiteSpace())
            {
                selectedType = null;
            }
        }

        // Display found types
        DisplayFoundTypes();

        DisplaySelectedType();
    }

    private void DisplaySelectedType()
    {
        // Selected Type
        if (selectedType != null)
        {
            GUILayout.Space(10);

            overridedSelectedTypeName = EditorGUILayout.TextField($"Type:", overridedSelectedTypeName);

            EditorGUILayout.LabelField("NameSpace:", selectedType.Namespace, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Type Full Name:", selectedType.FullName, EditorStyles.boldLabel);

            selectedTypeDefaultValue = EditorGUILayout.TextField("Default Value:", selectedTypeDefaultValue);

            EditorGUILayout.LabelField($"Drop Menu Order: {channelOrderInDropMenu}");

            filteredTypes = null;
            searchQuery = string.Empty;

            GUILayout.Space(10);

            // Create Button
            if (GUILayout.Button("Generate Channel/Listener"))
            {
                GenerateChannelClass();
                GenerateListenerClass();

                $"ScriptableObject '{selectedType.Name}EventChannel' and MonoBehaviour '{selectedType.Name}Listener' Generated.".Log(Color.white);
            }

            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5)); // Horizontal line
        }
    }

    private int GetLengthOfSubclasses()
    {
        System.Type[] subClassTypes = System.AppDomain.CurrentDomain.GetAssemblies()
              .SelectMany(s => s.GetTypes())
              .Where(p => p.IsClass && !p.IsAbstract && p.IsSubclassOfGeneric(typeof(GenericScriptableEventChannel<>)))
              .ToArray();

        return subClassTypes.Length;
    }

    private void OnSearchQueryChanged()
    {
        if (EditorApplication.timeSinceStartup - searchQueryLastChangeTime > searchQueryChangeDelay)
        {
            // This method will be called after a short delay when the user stops typing
            EditorApplication.update -= OnSearchQueryChanged;
            previousSearchQuery = searchQuery;
            searchQueryChangeCounter = 0;
            canSearch = true;
        }
    }

    private void DisplayFoundTypes()
    {
        if (canSearch)
        {
            if (searchQuery.IsNullOrWhiteSpace()) return;

            // Get all non-abstract classes and structs in the project
            System.Type[] types = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => (p.IsClass || p.IsEnum || p.IsAnsiClass) && !p.IsAbstract)
                .ToArray();

            // Filter types based on search query
            filteredTypes = types.Where(t => t.Name.ToLower().Contains(searchQuery.ToLower())).ToArray();

            canSearch = false;
        }

        if (filteredTypes != null && filteredTypes.Length > 0)
        {
            int loopLength = Mathf.Min(maxSearchLength, filteredTypes.Length);

            loopLength = Mathf.Max(0, loopLength);

            // Display the found types
            GUILayout.Label($"Found Types: {loopLength}", EditorStyles.boldLabel);
            // ScrollView for found types
            EditorGUILayout.BeginVertical();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < loopLength; i++)
            {
                System.Type type = filteredTypes[i];
                if (GUILayout.Button(type.Name))
                {
                    selectedType = type;
                    overridedSelectedTypeName = selectedType.Name;
                    channelOrderInDropMenu = GetLengthOfSubclasses();
                    GUI.FocusControl(null);
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }

    #region Generator

    private void GenerateChannelClass()
    {
        if (selectedType == null)
        {
            Debug.LogWarning("No type selected.");
            return;
        }

        // Create ScriptableObject.cs
        string scriptableObjectClassPath = GetChannelClassPath(selectedType.Name);
        string scriptableObjectCode = GenerateChannelScriptCodeScheme(
                                        _typeName: selectedType.Name,
                                        _type: overridedSelectedTypeName,
                                        _defaultValue: selectedTypeDefaultValue,
                                        _nameSpace: selectedType.Namespace,
                                        _order: channelOrderInDropMenu);

        WriteCodeInFolder(scriptableObjectClassPath, scriptableObjectCode);

        AssetDatabase.Refresh();
    }

    private void GenerateListenerClass()
    {
        if (selectedType == null)
        {
            Debug.LogWarning("No type selected.");
            return;
        }
        // Create ScriptableObject.cs
        string monoBehaviourClassPath = GetListenerClassPath(selectedType.Name);
        string monoBehaviourCode = GenerateListenerScriptCodeScheme(
                                        _typeName: selectedType.Name,
                                        _type: overridedSelectedTypeName,
                                        _nameSpace: selectedType.Namespace,
                                        _order: channelOrderInDropMenu);

        WriteCodeInFolder(monoBehaviourClassPath, monoBehaviourCode);

        AssetDatabase.Refresh();
    }

    private static string GetChannelClassPath(string _typeName)
    {
        return $"Assets/IA-ScriptableEventChannel/Channel/{_typeName}ValueEventChannel.cs";
    }

    private static string GetListenerClassPath(string _typeName)
    {
        return $"Assets/IA-ScriptableEventChannel/Listener/{_typeName}ValueEventListener.cs";
    }

    private static void WriteCodeInFolder(string monoBehaviourPath, string monoBehaviourCode)
    {
        // Check if the directory exists, create if not
        string directoryPath = System.IO.Path.GetDirectoryName(monoBehaviourPath);
        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }

        // Write or overwrite the file
        System.IO.File.WriteAllText(monoBehaviourPath, monoBehaviourCode);
    }

    private static string GenerateChannelScriptCodeScheme(string _typeName, string _type, string _defaultValue, string _nameSpace = "", int _order = 0)
    {
        StringBuilder codeBuilder = new StringBuilder();

        if (!_nameSpace.IsNullOrWhiteSpace() && _nameSpace.CompareTo("UnityEngine") != 0)
        {
            codeBuilder.AppendLine($"using {_nameSpace};");
        }

        codeBuilder.AppendLine("using UnityEngine;");
        codeBuilder.AppendLine();
        codeBuilder.AppendLine("namespace IA.ScriptableEvent.Channel");
        codeBuilder.AppendLine("{");
        codeBuilder.AppendLine($"\t[CreateAssetMenu(fileName = \"{_typeName} Value Event Channel\", menuName = \"IA/Event Channel/ -> {_typeName} Value Event Channel\", order = {_order})]");
        codeBuilder.AppendLine($"\tpublic class {_typeName}ValueEventChannel : GenericScriptableEventChannel<{_type}>");
        codeBuilder.AppendLine("\t{");
        codeBuilder.AppendLine($"\t\tpublic override void LoadDefaultData() => value = {_defaultValue};");
        codeBuilder.AppendLine("\t}");
        codeBuilder.AppendLine("}");

        return codeBuilder.ToString();
    }

    private static string GenerateListenerScriptCodeScheme(string _typeName, string _type, string _nameSpace = "", int _order = 0)
    {
        StringBuilder codeBuilder = new StringBuilder();

        if (!_nameSpace.IsNullOrWhiteSpace() && _nameSpace.CompareTo("UnityEngine") != 0)
        {
            codeBuilder.AppendLine($"using {_nameSpace};");
        }

        codeBuilder.AppendLine("using UnityEngine;");
        codeBuilder.AppendLine();
        codeBuilder.AppendLine("#if UNITY_EDITOR");
        codeBuilder.AppendLine("using UnityEditor;");
        codeBuilder.AppendLine("#endif");
        codeBuilder.AppendLine();
        codeBuilder.AppendLine("namespace IA.ScriptableEvent.Listener");
        codeBuilder.AppendLine("{");
        codeBuilder.AppendLine($"\tpublic class {_typeName}ChannelListener : GenericScriptableEventChannelListener<{_type}>");
        codeBuilder.AppendLine("\t{");
        codeBuilder.AppendLine("\t\t// Add your specific functionality here");
        codeBuilder.AppendLine("#if UNITY_EDITOR");
        codeBuilder.AppendLine();
        codeBuilder.AppendLine($"\t\t[MenuItem(\"GameObject/IA/Event Listener/ -> {_typeName} Channel Listener\", false, {_order})]");
        codeBuilder.AppendLine("\t\tpublic static void AddListenerToHierarchy()");
        codeBuilder.AppendLine("\t\t{");
        codeBuilder.AppendLine($"\t\t\tGameObject listenerObj = new GameObject(\"{_typeName} Channel Listener\");");
        codeBuilder.AppendLine($"\t\t\tlistenerObj.AddComponent<{_typeName}ChannelListener>();");
        codeBuilder.AppendLine("\t\t\tSelection.activeGameObject = listenerObj;");
        codeBuilder.AppendLine("\t\t}");
        codeBuilder.AppendLine("#endif");
        codeBuilder.AppendLine("\t}");
        codeBuilder.AppendLine("}");

        return codeBuilder.ToString();
    }

    #endregion

}
#endif