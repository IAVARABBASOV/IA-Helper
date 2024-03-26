using UnityEngine;
using System;

#if UNITY_EDITOR
using IA.Attributes;
#endif

namespace IA.JsonManager.Scriptable
{
    [Serializable]
    public class JsonScriptableData
    {
        [SerializeField]
#if UNITY_EDITOR
        [ReadOnly(0.5f, a: 0.15f)]
#endif
        private string name;
#if UNITY_EDITOR
        [ReadOnly(0.5f, 1f, 0.5f, 0.25f)]
#endif
        [TextArea]
        public string JsonData;

        public JsonScriptableData(string _name)
        {
            name = _name;
        }
    }
}