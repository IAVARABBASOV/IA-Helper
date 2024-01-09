#if UNITY_EDITOR
using IA.Attributes;
#endif

using UnityEngine;
using UnityEngine.Events;
using IA.Utils;
using IA.JsonManager.Scriptable;
using IA.ScriptableEvent.Listener;
using System.Collections.Generic;

namespace IA.ScriptableEvent.Channel
{
    public abstract class GenericScriptableEventChannel<T> : BaseJsonScriptable
    {
        [SerializeField] protected T value;
        [SerializeField] protected UnityEvent<T> unityEvent;
        protected List<IChannelListener<T>> listeners = new List<IChannelListener<T>>();

        public T GetValue => value;
        public UnityEvent<T> GetUnityEvent => unityEvent;

        public GenericScriptableEventChannel<T> SetValue(T _value) { value = _value; return this; }
        public void AddListener(IChannelListener<T> _listener) => listeners.Add(_listener);
        public void RemoveListener(IChannelListener<T> _listener) => listeners.Remove(_listener);
        public bool ListenerExist(IChannelListener<T> _listener) => listeners.Exists(x => GameObject.ReferenceEquals(x, _listener));

        public void RemoveAllListeners() { listeners.Clear(); unityEvent.RemoveAllListeners(); }

        public void RaiseEvent()
        {
            foreach (IChannelListener<T> _listener in listeners) _listener.InvokeResponse(value);

            unityEvent?.Invoke(value);
        }

        public override string GetJsonString()
        {
            return DataSaveLoad.ConvertToJsonText(this, (jObject) =>
            {
                // Don't add unityEvent into json text
                jObject.Remove(nameof(unityEvent));
                jObject.Remove("isDebugEnabled");

                return jObject;
            });
        }

#if UNITY_EDITOR

        [SerializeField] private bool isDebugEnabled = false;

        private void OnValidate()
        {
            CallEventFromEditor();
        }

        [InspectorButton("Call Event Manually")]
        public void CallEventFromEditor()
        {
            RaiseEvent();
            if (isDebugEnabled)
            {
                int listenersCount = listeners.Count;
                $"{name}: Value = {value}\nRaise Event Called from Editor! <color=orange>-> Listeners Count: {listenersCount}</color>".Log(_context: this);
            }
        }
#endif
    }
}