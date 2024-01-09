using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using IA.Utils;

namespace IA.LevelSystem.Utils
{
    public class LevelMakerItemInstance : MonoBehaviour
    {
        #region References

        public Transform ThisTransform;

        public Component targetComponent;

        #endregion

        #region Events

        public event BehaviourHandler OnStart;
        public event BehaviourHandler OnFixedUpdate;
        public event BehaviourHandler OnUpdate;
        public event BehaviourHandler OnLateUpdate;
        public event BehaviourHandler OnDestroyed;

        public delegate void BehaviourHandler(LevelMakerItemInstance target);

        #endregion

        #region Unity Functions
        protected virtual void Awake()
        {
            ThisTransform = transform;
        }

        protected virtual void Start()
        {
            OnStart?.Invoke(this);
        }

        protected virtual void FixedUpdate()
        {
            OnFixedUpdate?.Invoke(this);
        }

        protected virtual void Update()
        {
            OnUpdate?.Invoke(this);
        }

        protected virtual void LateUpdate()
        {
            OnLateUpdate?.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }
        #endregion

        #region Other Functions
        public void DestroyMe(float inSec)
        {
            Destroy(gameObject, inSec);
        }
        #endregion
    }
}