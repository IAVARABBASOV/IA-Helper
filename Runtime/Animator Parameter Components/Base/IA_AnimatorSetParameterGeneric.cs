using System.Collections;
using UnityEngine;

namespace IA.AnimatorParameter
{
    [RequireComponent(typeof(Animator)), ExecuteAlways]
    public abstract class IA_AnimatorSetParameterGeneric<T> : MonoBehaviour
    {
        [SerializeField] protected Animator anim = null;

        [Space(10)]
        [SerializeField, AnimatorParameter("anim", "editorAnimParamType")] protected string key;
        [SerializeField] private T value;

        [Space(10)]
        [SerializeField] protected bool setOnStart = false;
        [SerializeField] protected bool setOnEnable = false;

        [HideInInspector] public AnimatorControllerParameterType editorAnimParamType;
        public abstract AnimatorControllerParameterType GetParameterType { get; }

        protected void Awake()
        {
            anim = GetComponent<Animator>();
            editorAnimParamType = GetParameterType;
        }

        protected void Start()
        {
            if (Application.isPlaying)
            {
                if (setOnStart)
                {
                    SetValue();
                }
            }
        }

        protected void OnEnable()
        {
            if (Application.isPlaying)
            {
                if (setOnEnable)
                {
                    SetValue();
                }
            }
        }

        private void SetValue()
        {
            if (GetParameterType != AnimatorControllerParameterType.Trigger)
            {
                SetValue(value);
            }
            else
            {
                SetTrigger();
            }
        }

        public void SetTrigger() => anim.SetTrigger(key);

        public abstract void SetValue(T value);
    }
}