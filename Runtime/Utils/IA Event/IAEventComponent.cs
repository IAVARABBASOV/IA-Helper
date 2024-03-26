using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IA.Utils
{
    public class IAEventComponent : MonoBehaviour
    {
        public UnityEvent OnEvent;

        public List<UnityEvent> MultiEvents = new List<UnityEvent>();

        [SerializeField] private int index;

        public void DefineEventIndex(int _index) => index = _index;

        public void CallDefinedEvent()
        {
            if (index >= 0 || index <= MultiEvents.Count - 1)
            {
                MultiEvents[index].Invoke();
            }
        }

        public void CallEvent()
        {
            if (gameObject.activeSelf)
                OnEvent?.Invoke();
        }

        public void CallMultiEvent(int index)
        {
            if (index >= 0 || index <= MultiEvents.Count - 1)
            {
                MultiEvents[index].Invoke();
            }
        }
    }
}