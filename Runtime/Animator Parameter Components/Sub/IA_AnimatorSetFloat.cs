using UnityEngine;

namespace IA.AnimatorParameter
{
    public class IA_AnimatorSetFloat : IA_AnimatorSetParameterGeneric<float>
    {
        public override AnimatorControllerParameterType GetParameterType => AnimatorControllerParameterType.Float;

        public override void SetValue(float value) => anim.SetFloat(key, value);
    }
}