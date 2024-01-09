using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IA.ScriptableEvent.Listener
{
	public class StringChannelListener : GenericScriptableEventChannelListener<string>
	{
		// Add your specific functionality here
#if UNITY_EDITOR

		[MenuItem("GameObject/IA/Event Listener/ -> String Channel Listener", false, 1)]
		public static void AddListenerToHierarchy()
		{
			GameObject listenerObj = new GameObject("String Channel Listener");
			listenerObj.AddComponent<StringChannelListener>();
			Selection.activeGameObject = listenerObj;
		}
#endif
	}
}
