using System;
using UnityEngine;

namespace IA.ScriptableEvent.Channel
{
	[CreateAssetMenu(fileName = "Single Value Event Channel", menuName = "IA/Event Channel/ -> Single Value Event Channel", order = 2)]
	public class SingleValueEventChannel : GenericScriptableEventChannel<float>
	{
		public override void LoadDefaultData() => value = 0.0f;
	}
}
