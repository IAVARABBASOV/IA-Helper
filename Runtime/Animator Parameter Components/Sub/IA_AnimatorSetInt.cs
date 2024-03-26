using UnityEngine;

namespace IA.AnimatorParameter
{
    public class IA_AnimatorSetInt : IA_AnimatorSetParameterGeneric<int>
    {
        public override AnimatorControllerParameterType GetParameterType => AnimatorControllerParameterType.Int;

        public override void SetValue(int value) => anim.SetInteger(key, value);
    }
}