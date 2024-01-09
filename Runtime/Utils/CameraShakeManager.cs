using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Events;

namespace IA.Utils
{
    public class CameraShakeManager : Singleton<CameraShakeManager>
    {
        [Header("References")]
        public CinemachineVirtualCamera CinemachineCamera = null;

        [Header("Events")]
        public UnityEvent OnCameraShakeCompleted;
        public event UnityAction OnCameraShakeCompletedEvent;

        private CinemachineBasicMultiChannelPerlin vibration;

        private bool isCameraShake;

        protected override void Awake()
        {
            vibration = CinemachineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            base.Awake();
        }

        public void CameraShake(float duration = 0.5f, float amplitude = 1f, float frequency = 1f)
        {
            CinemachineCamera.transform.DOComplete();

            if (isCameraShake) return;

            ShakeRoutine(duration, amplitude, frequency).StartCoroutine(this);

            isCameraShake = true;
        }

        private IEnumerator ShakeRoutine(float duration, float amplitude, float frequency)
        {
            vibration.m_AmplitudeGain = amplitude;
            vibration.m_FrequencyGain = frequency;

            yield return new WaitForSecondsRealtime(duration);

            vibration.m_AmplitudeGain = 0;
            vibration.m_FrequencyGain = 0;
            isCameraShake = false;

            OnCameraShakeCompletedEvent?.Invoke();
            OnCameraShakeCompleted?.Invoke();
        }
    }
}