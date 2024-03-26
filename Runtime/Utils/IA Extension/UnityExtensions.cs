using UnityEngine;
using System.Text;
using UnityEditor;

namespace IA.Utils
{
    public static class UnityExtensions
    {
#if UNITY_EDITOR
        public static string GetCurrentNamingSchemeValue(int namingIndex = 1)
        {
            int digitsMaxIndex = UnityEditor.EditorSettings.gameObjectNamingDigits - 1;

            switch (UnityEditor.EditorSettings.gameObjectNamingScheme)
            {
                case EditorSettings.NamingScheme.SpaceParenthesis:
                    {
                        StringBuilder namingScheme = new StringBuilder($" ({namingIndex})");

                        for (int i = 0; i < digitsMaxIndex; i++)
                        {
                            namingScheme.Insert(2, "0");
                        }

                        return namingScheme.ToString();
                    }
                case EditorSettings.NamingScheme.Dot:
                    {
                        StringBuilder namingScheme = new StringBuilder($".{namingIndex}");

                        for (int i = 0; i < digitsMaxIndex; i++)
                        {
                            namingScheme.Insert(1, "0");
                        }

                        return namingScheme.ToString();
                    }
                case EditorSettings.NamingScheme.Underscore:
                    {
                        StringBuilder namingScheme = new StringBuilder($"_{namingIndex}");

                        for (int i = 0; i < digitsMaxIndex; i++)
                        {
                            namingScheme.Insert(1, "0");
                        }

                        return namingScheme.ToString();
                    }
                default:
                    break;
            }

            return $" ({namingIndex})";
        }

        public static int GetCurrentNamingSchemeCount()
        {
            string namingScheme = GetCurrentNamingSchemeValue();

            return namingScheme.Length;
        }

        /// <summary>
        /// Can check the GameObject Dublicated or Not by its Name
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CheckIsDublicated(this GameObject target)
        {
            string currentNamingScheme;

            bool isDublicated = false;

            for (int i = 1; i < 9; i++)
            {
                currentNamingScheme = GetCurrentNamingSchemeValue(i);

                /// Check name Contain naming scheme this mean is this GameObject is dublicated
                if (target.name.Contains(currentNamingScheme))
                {
                    isDublicated = true;

                    break;
                }
            }

            return isDublicated;
        }

        /// <summary>
        /// Remove the (1) from GameObject Name
        /// </summary>
        /// <param name="target"></param>
        public static void RemoveSchemeName(this GameObject target)
        {
            StringBuilder nameString = new StringBuilder(target.name);

            /// Remove Unity GameObject Naming extension from Name
            int namingSchemeCount = GetCurrentNamingSchemeCount();
            int removeStartIndex = nameString.Length - namingSchemeCount;
            nameString.Remove(removeStartIndex, namingSchemeCount);

            target.name = nameString.ToString();
        }

        public static Transform CreateTransform(string _name, Vector3 _pos, Quaternion _rot, Vector3 _scale, Transform parent)
        {
            GameObject boundsOriginInstance = new GameObject(_name);
            boundsOriginInstance.transform.SetParent(parent);

            boundsOriginInstance.transform.localPosition = _pos;
            boundsOriginInstance.transform.localRotation = _rot;
            boundsOriginInstance.transform.localScale = _scale;

            return boundsOriginInstance.transform;
        }

        public static bool IsOriginalPrefabInAssets(this GameObject _go)
        => PrefabUtility.GetPrefabAssetType(_go) != PrefabAssetType.NotAPrefab
        && PrefabUtility.GetPrefabInstanceStatus(_go) == PrefabInstanceStatus.NotAPrefab;

        public static bool IsPrefabInstanceInHierarchy(this GameObject _go)
        => PrefabUtility.GetPrefabAssetType(_go) != PrefabAssetType.NotAPrefab
        && PrefabUtility.GetPrefabInstanceStatus(_go) != PrefabInstanceStatus.NotAPrefab;

#endif
    }
}