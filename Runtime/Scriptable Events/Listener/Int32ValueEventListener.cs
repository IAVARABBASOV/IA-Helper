using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IA.ScriptableEvent.Listener
{
	public class Int32ChannelListener : GenericScriptableEventChannelListener<int>
	{
		// Add your specific functionality here
#if UNITY_EDITOR

		[MenuItem("GameObject/IA/Event Listener/ -> Int32 Channel Listener", false, 0)]
		public static void AddListenerToHierarchy()
		{
			GameObject listenerObj = new GameObject("Int32 Channel Listener");
			listenerObj.AddComponent<Int32ChannelListener>();
			Selection.activeGameObject = listenerObj;
		}
#endif
	}
}
