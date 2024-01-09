using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IA.ScriptableEvent.Listener
{
	public class SingleChannelListener : GenericScriptableEventChannelListener<float>
	{
		// Add your specific functionality here
#if UNITY_EDITOR

		[MenuItem("GameObject/IA/Event Listener/ -> Single Channel Listener", false, 2)]
		public static void AddListenerToHierarchy()
		{
			GameObject listenerObj = new GameObject("Single Channel Listener");
			listenerObj.AddComponent<SingleChannelListener>();
			Selection.activeGameObject = listenerObj;
		}
#endif
	}
}
