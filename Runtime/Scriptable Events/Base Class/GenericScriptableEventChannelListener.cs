using IA.ScriptableEvent.Channel;
using IA.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace IA.ScriptableEvent.Listener
{
    [ExecuteAlways]
    public abstract class GenericScriptableEventChannelListener<T> : MonoBehaviour, IChannelListener<T>
    {
        [SerializeField] protected GenericScriptableEventChannel<T> targetEventChannel;
        [SerializeField] protected UnityEvent<T> eventResponse;

        [SerializeField, Tooltip("Get event Response On Enable")] protected bool callback_OnEnable = false;
        [SerializeField, Tooltip("Get event Response On Start")] protected bool callback_OnStart = true;


        protected virtual void Start()
        {
            if (targetEventChannel && callback_OnStart)
            {
                InvokeResponse(targetEventChannel.GetValue);
            }
        }

        protected virtual void OnEnable()
        {
            if (targetEventChannel != null)
            {
                SetEventChannelListener();

                if (callback_OnEnable)
                {
                    InvokeResponse(targetEventChannel.GetValue);
                }
            }

            //  "On Enable Called".LogError(_color: Color.green, _context: this);
        }

        protected virtual void OnDisable()
        {
            if (targetEventChannel != null)
            {
                RemoveEventChannelListener();
            }

            //  "On Disabled Called".LogError(_color: Color.yellow, _context: this);
        }

        protected virtual void OnDestroy()
        {
            if (targetEventChannel != null)
            {
                RemoveEventChannelListener();
            }

            //    "On Destroy Called".LogError(_color: Color.red, _context: this);
        }

        private void SetEventChannelListener()
        {
            /// Add the listener to the event channel list while it is not exist in there
            bool isExistInListenerList = targetEventChannel.ListenerExist(this);

            if (!isExistInListenerList)
                targetEventChannel.AddListener(this);
        }

        private void RemoveEventChannelListener() => targetEventChannel.RemoveListener(this);

        public virtual void InvokeResponse(T targetType) => eventResponse?.Invoke(targetType);
        public virtual void AddListener(UnityAction<T> e) => eventResponse.AddListener(e);
        public virtual void RemoveListener(UnityAction<T> e) => eventResponse.RemoveListener(e);

#if UNITY_EDITOR
        protected GenericScriptableEventChannel<T> previousTargetEventChannel;

        protected virtual void OnValidate()
        {
            /// Start listening to Event when event channel is not null
            if (targetEventChannel != null)
            {
                SetEventChannelListener();

                previousTargetEventChannel = targetEventChannel;
            }

            /// Remove listener (me) from event channel when user remove it from reference field
            if (targetEventChannel == null)
            {
                if (previousTargetEventChannel != null)
                {
                    previousTargetEventChannel.RemoveListener(this);
                    previousTargetEventChannel = null;
                }
            }
        }

#endif
    }
}