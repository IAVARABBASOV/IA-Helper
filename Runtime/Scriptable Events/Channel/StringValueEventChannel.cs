using UnityEngine;

namespace IA.ScriptableEvent.Channel
{
	[CreateAssetMenu(fileName = "String Value Event Channel", menuName = "IA/Event Channel/ -> String Value Event Channel", order = 1)]
	public class StringValueEventChannel : GenericScriptableEventChannel<string>
	{
		public override void LoadDefaultData() => value = string.Empty;
	}
}
