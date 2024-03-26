#if UNITY_EDITOR

using UnityEditor.AddressableAssets.Settings;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using System.Collections.Generic;

namespace IA.Utils
{
    public static class AddressableEditorExtension
    {
        public static bool IsAddressableAsset(this UnityEngine.Object obj)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Addressable Assets did not Created yet");

                return false;
            }
            AddressableAssetEntry entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj)));
            return entry != null;
        }

        public static AddressableAssetEntry GetAddressableAssetEntry(this UnityEngine.Object obj)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Addressable Assets did not Created yet");

                return null;
            }
            return settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj)));
        }

        /// <summary>
        /// Set Addressables Key/ID of an gameObject.
        /// </summary>
        /// <param name="gameObject">GameObject to set Key/ID</param>
        /// <param name="id">Key/ID</param>
        public static void SetAddressableID(this GameObject gameObject, string id)
        {
            SetAddressableID(gameObject as Object, id);
        }

        /// <summary>
        /// Set Addressables Key/ID of an object.
        /// </summary>
        /// <param name="o">Object to set Key/ID</param>
        /// <param name="id">Key/ID</param>
        public static void SetAddressableID(this Object o, string id)
        {
            if (id.Length == 0)
            {
                Debug.LogWarning($"Can not set an empty adressables ID.");
            }
            AddressableAssetEntry entry = CreateAddressableAssetEntry(o);
            if (entry != null)
            {
                entry.address = id;
            }
        }

        /// <summary>
        /// Get Addressables Key/ID of an gameObject.
        /// </summary>
        /// <param name="gameObject">gameObject to recive addressables Key/ID</param>
        /// <returns>Addressables Key/ID</returns>
        public static string GetAddressableID(this GameObject gameObject)
        {
            return GetAddressableID(gameObject as Object);
        }

        /// <summary>
        /// Get Addressables Key/ID of an object.
        /// </summary>
        /// <param name="o">object to recive addressables Key/ID</param>
        /// <returns>Addressables Key/ID</returns>
        public static string GetAddressableID(this Object o)
        {
            AddressableAssetEntry entry = CreateAddressableAssetEntry(o);
            if (entry != null)
            {
                return entry.address;
            }
            return "";
        }

        /// <summary>
        /// Get addressable asset entry of an object.
        /// </summary>
        /// <param name="o">>object to recive addressable asset entry</param>
        /// <returns>addressable asset entry</returns>
        public static AddressableAssetEntry CreateAddressableAssetEntry(this Object o)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Addressable Assets did not Created yet");

                return null;
            }
            var group = settings.DefaultGroup;
            var entriesAdded = new List<AddressableAssetEntry>();

            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(o));

            var entry = settings.CreateOrMoveEntry(guid, group, readOnly: false, postEvent: false);
            entry.address = AssetDatabase.GUIDToAssetPath(guid);

            entriesAdded.Add(entry);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);

            return entry;
        }

        public static AddressableAssetEntry GetEntryByAddress(string address)
        {
            // string guid = AssetDatabase.AssetPathToGUID(address);
            // var settings = AddressableAssetSettingsDefaultObject.Settings;
            // return settings.FindAssetEntry(guid);

            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings == null)
            {
                Debug.LogError("Addressable Assets did not Created yet");

                return null;
            }

            var groups = settings.groups;
            foreach (var group in groups)
            {
                foreach (var entry in group.entries)
                {
                    if (entry.address == address) return entry;
                }
            }

            return null;
        }
    }
}
#endif