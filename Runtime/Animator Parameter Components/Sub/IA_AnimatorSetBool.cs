using UnityEngine;

namespace IA.AnimatorParameter
{
    public class IA_AnimatorSetBool : IA_AnimatorSetParameterGeneric<bool>
    {
        public override AnimatorControllerParameterType GetParameterType => AnimatorControllerParameterType.Bool;

        public override void SetValue(bool value) => anim.SetBool(key, value);
    }
}