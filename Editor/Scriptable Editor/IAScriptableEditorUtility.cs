#if UNITY_EDITOR

using UnityEditor;
using System.Linq;
using UnityEngine.Events;

namespace IA.Scriptable.Editor
{
    public static class IAScriptableEditorUtility
    {
        public static string[] GetSubclassNames(System.Type[] subClassTypes)
        {
            string[] subClassTypeNames = new string[subClassTypes.Length];
            for (int i = 0; i < subClassTypes.Length; i++)
            {
                subClassTypeNames[i] = subClassTypes[i].FullName;
            }

            return subClassTypeNames;
        }

        public static System.Type[] GetSubclasTypes(System.Type rewardType)
        {
            return System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => rewardType.IsAssignableFrom(type) && !type.IsAbstract)
                .ToArray();
        }

        public static string GetClassNameFromFullName(string typeName, string _targetNamespaceToClassNameStart)
        {
            return typeName.Replace(_targetNamespaceToClassNameStart, "");
        }

        public static GenericMenu GetGenericMenu(string[] subClassNames, UnityAction<string, GenericMenu> e)
        {
            GenericMenu genericMenu = new GenericMenu();

            foreach (var item in subClassNames)
            {
                e.Invoke(item, genericMenu);
            }

            return genericMenu;
        }
    }
}

#endif