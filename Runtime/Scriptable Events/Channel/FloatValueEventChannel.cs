using UnityEngine;

namespace IA.ScriptableEvent.Channel
{
	[CreateAssetMenu(fileName = "Float Value Event Channel", menuName = "IA/Event Channel/ -> Float Value Event Channel", order = 2)]
	public class FloatValueEventChannel : GenericScriptableEventChannel<float>
	{
		public override void LoadDefaultData() => value = 0.0f;
	}
}
