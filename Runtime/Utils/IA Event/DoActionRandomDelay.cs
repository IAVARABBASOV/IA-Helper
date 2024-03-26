using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IA.Utils
{
    public class DoActionRandomDelay : MonoBehaviour
    {
        public bool CallOnStart = true;
        public float DelayMin, DelayMax;

        public UnityEvent OnEvent;

        private void Start()
        {
            if (CallOnStart)
            {
                StartEventCoroutine();
            }
        }

        public void StartEventCoroutine()
        {
            float randomDelay = Random.Range(DelayMin, DelayMax);

            if (gameObject.activeSelf)
            {
                new WaitForSeconds(randomDelay).EventRoutine(() =>
                {
                    if (gameObject.activeSelf)
                    {
                        OnEvent?.Invoke();
                    }
                }).
                StartCoroutine(this);
            }
        }
    }
}