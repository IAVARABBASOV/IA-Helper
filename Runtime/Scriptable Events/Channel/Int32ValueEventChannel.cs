using System;
using UnityEngine;

namespace IA.ScriptableEvent.Channel
{
	[CreateAssetMenu(fileName = "Int32 Value Event Channel", menuName = "IA/Event Channel/ -> Int32 Value Event Channel", order = 0)]
	public class Int32ValueEventChannel : GenericScriptableEventChannel<int>
	{
		public override void LoadDefaultData() => value = 0;
	}
}
