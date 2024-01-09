using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static CartoonFX.CFXR_Effect;

namespace IA.Utils
{
    public class DestroyedItemSlowMotionManager : Singleton<DestroyedItemSlowMotionManager>
    {
        [Header("Slow Motion Property")]
        [SerializeField] private float slowMotionCoolDown = 5f;
        [SerializeField] private float slowMotionLength = 1f;

        [Header("Camera Shake Property")]
        [SerializeField] private float cameraShakeDuration = 0.3f;
        [SerializeField] private float cameraShakeAmplitude = 3f;
        [SerializeField] private float cameraShakeFrequency = 3f;

        private bool isCameraShake = false;

        private float slowMotionTime;

        public static void TrySlowMotion()
        {
            MakeCameraShake();
        }

        private static void MakeCameraShake()
        {
            if (!Instance.isCameraShake)
            {
                Instance.isCameraShake = true;

                CameraShakeManager.Instance.OnCameraShakeCompletedEvent += OnCameraShakeCompleted;
                CameraShakeManager.Instance.CameraShake(Instance.cameraShakeDuration, Instance.cameraShakeAmplitude, Instance.cameraShakeFrequency);
            }
        }

        private static void OnCameraShakeCompleted()
        {
            CameraShakeManager.Instance.OnCameraShakeCompletedEvent -= OnCameraShakeCompleted;
            Instance.isCameraShake = false;

            TryDoSlowMotion();
        }

        private static void TryDoSlowMotion()
        {
            /// Is time to slow motion
            if (Time.time > Instance.slowMotionTime)
            {
                /// Set Next time to slow motion
                Instance.slowMotionTime = Time.time + Instance.slowMotionCoolDown;

                SlowMotionComponent.Instance.MakeSlowMotion(Instance.slowMotionLength);
            }
        }
    }
}