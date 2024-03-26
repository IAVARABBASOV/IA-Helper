using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IA.ScriptableEvent.Listener
{
	public class FloatValueChannelListener : GenericScriptableEventChannelListener<float>
	{
		// Add your specific functionality here
#if UNITY_EDITOR

		[MenuItem("GameObject/IA/Event Listener/ -> Float Channel Listener", false, 2)]
		public static void AddListenerToHierarchy()
		{
			GameObject listenerObj = new GameObject("Float Channel Listener");
			listenerObj.AddComponent<FloatValueChannelListener>();
			Selection.activeGameObject = listenerObj;
		}
#endif
	}
}
