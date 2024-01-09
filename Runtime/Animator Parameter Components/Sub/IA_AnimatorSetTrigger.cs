using UnityEngine;

namespace IA.AnimatorParameter
{
    public class IA_AnimatorSetTrigger : IA_AnimatorSetParameterGeneric<int>
    {
        public override AnimatorControllerParameterType GetParameterType => AnimatorControllerParameterType.Trigger;

        public override void SetValue(int value) => anim.SetTrigger(key);
    }
}