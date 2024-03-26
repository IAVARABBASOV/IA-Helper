using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IA.AnimatorParameter
{
    public class AnimatorParameterAttribute : PropertyAttribute
    {
        public string AnimatorReferenceFieldName { get; }
        public string ParameterTypeFieldName { get; }
        public AnimatorControllerParameterType parameterType;

        public AnimatorParameterAttribute(string _animatorFieldName)
        {
            AnimatorReferenceFieldName = _animatorFieldName;
            ParameterTypeFieldName = string.Empty;
        }

        public AnimatorParameterAttribute(string _animatorFieldName, string _parameterType)
        {
            AnimatorReferenceFieldName = _animatorFieldName;
            ParameterTypeFieldName = _parameterType;
        }

        public AnimatorParameterAttribute(string _animatorFieldName, AnimatorControllerParameterType _parameterType)
        {
            AnimatorReferenceFieldName = _animatorFieldName;
            parameterType = _parameterType;
        }
    }
}
