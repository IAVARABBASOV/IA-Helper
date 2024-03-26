using UnityEngine;

namespace IA.Utils
{
    public class CameraSize : MonoBehaviour
    {
        [SerializeField] private Camera cam = null;

        [SerializeField] private Cinemachine.CinemachineVirtualCamera vCam = null;

        public float sceneWidth = 25;

        protected void Update()
        {
            float unitsPerPixel = sceneWidth / Screen.width;

            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

            if (cam)
            {
                if (!cam.orthographic) cam.fieldOfView = desiredHalfHeight;
                else cam.orthographicSize = desiredHalfHeight;
            }

            if (vCam)
            {
                if (vCam.m_Lens.Orthographic)
                {
                    vCam.m_Lens.OrthographicSize = desiredHalfHeight;
                }
                else
                {
                    vCam.m_Lens.FieldOfView = desiredHalfHeight;
                }
            }
        }

    }
}
