using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IA.Utils
{
    public class TrajectoryController : Singleton<TrajectoryController>
    {
        public int maxIterations;
        public float pL_SmoothFadeOutDurationInSec = 0.02f;
        public float pl_SmoothFadeOutStartDurationInSec = 0.5f;
        public float quality = 0.5f;

        public Gradient startColor;
        public Gradient completedColor;

        LineRenderer lineRenderer;

        void Start()
        {
            transform.parent = null;

            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = false;

            lineRenderer.colorGradient = startColor;
        }

        /*    public void predict(GameObject subject, Vector3 currentPosition, Vector3 force)
            {
                if (currentPhysicsScene.IsValid() && predictionPhysicsScene.IsValid())
                {
                    GameObject dummy = Instantiate(subject);
                    SceneManager.MoveGameObjectToScene(dummy, predictionScene);

                    dummy.transform.position = currentPosition;
                    dummy.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                    GameObject instanceTempLineRendererGMO = new GameObject("TEMP Line Renderer");
                    LineRenderer instanceLineRenderer = instanceTempLineRendererGMO.AddComponent<LineRenderer>();
                    instanceLineRenderer.positionCount = 0;
                    instanceLineRenderer.positionCount = maxIterations;

                    CopyLinerendererToInstance(instanceLineRenderer);

                    List<Vector3> linePositionsList = new List<Vector3>();

                    for (int i = 0; i < maxIterations; i++)
                    {
                        predictionPhysicsScene.Simulate(Time.fixedDeltaTime);
                        instanceLineRenderer.SetPosition(i, dummy.transform.position);
                        linePositionsList.Add(dummy.transform.position);
                    }

                    StartCoroutine(PlayTrajectoryFadeOutRoutine(instanceLineRenderer, linePositionsList));

                    Destroy(dummy);
                }
            }*/

        public LineRenderer CreateTempLineRenderer()
        {
            LineRenderer _instanceLineRenderer;

            GameObject instanceTempLineRendererGMO = new GameObject("TEMP Line Renderer");
            _instanceLineRenderer = instanceTempLineRendererGMO.AddComponent<LineRenderer>();
            _instanceLineRenderer.positionCount = 0;
            _instanceLineRenderer.positionCount = maxIterations;
            CopyLinerendererToInstance(_instanceLineRenderer);

            return _instanceLineRenderer;
        }

        public List<Vector3> SimulateTrajectory(LineRenderer _instanceLineRenderer, Vector3 _startPosition, Vector3 _force, LayerMask _layerMask, UnityAction<RaycastHit> _onHit = null)
        {
            float t = Time.fixedDeltaTime * quality;

            List<Vector3> linePositionsList = new List<Vector3>();

            Vector3 trajectoryPoint;

            for (int i = 0; i < maxIterations; i++)
            {
                float time = t * i / (float)(lineRenderer.positionCount);
                trajectoryPoint = _startPosition + _force * time + 0.5f * Physics.gravity * time * time;

                linePositionsList.Add(trajectoryPoint);
            }

            for (int i = 1; i <= 4; i++)
            {
                int lineIndex = i * 10;
                int nextLineIndex = lineIndex + 9;

                Vector3 startPoint = linePositionsList[lineIndex];
                Vector3 endPoint = linePositionsList[nextLineIndex];

                RaycastHit hit;
                if (Physics.Raycast(startPoint, endPoint - startPoint, out hit, (endPoint - startPoint).magnitude, _layerMask))
                {
                    _onHit?.Invoke(hit);
                    break;
                }
            }

            _instanceLineRenderer.positionCount = maxIterations;
            _instanceLineRenderer.SetPositions(linePositionsList.ToArray());


            return linePositionsList;
        }

        public void ChangeTrajectoryColorToRed(LineRenderer instanceLineRenderer)
        {
            instanceLineRenderer.colorGradient = completedColor;
        }

        public void TrajectoryFadeOut(LineRenderer instanceLineRenderer, List<Vector3> linePositionsList)
        {
            StartCoroutine(PlayTrajectoryFadeOutRoutine(instanceLineRenderer, linePositionsList));
        }

        private IEnumerator PlayTrajectoryFadeOutRoutine(LineRenderer instanceLineRenderer, List<Vector3> linePositionsList)
        {
            yield return new WaitForSeconds(pl_SmoothFadeOutStartDurationInSec);

            if (instanceLineRenderer != null)
            {
                int maxPositionCount = instanceLineRenderer.positionCount - 2;
                int currentPositionIndex = 0;

                float _smoothFadeOut = pL_SmoothFadeOutDurationInSec * Time.deltaTime;

                while (currentPositionIndex <= maxPositionCount)
                {
                    if (linePositionsList.Count > 0 && instanceLineRenderer != null)
                    {
                        linePositionsList.RemoveAt(0);
                        instanceLineRenderer.positionCount = linePositionsList.Count;
                        instanceLineRenderer.SetPositions(linePositionsList.ToArray());
                        yield return new WaitForSeconds(_smoothFadeOut);
                        currentPositionIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (instanceLineRenderer) Destroy(instanceLineRenderer.gameObject);
        }

        private void CopyLinerendererToInstance(LineRenderer instanceLineRenderer)
        {
            if (instanceLineRenderer == null) return;

            instanceLineRenderer.alignment = lineRenderer.alignment;
            instanceLineRenderer.allowOcclusionWhenDynamic = lineRenderer.allowOcclusionWhenDynamic;
            instanceLineRenderer.colorGradient = lineRenderer.colorGradient;
            instanceLineRenderer.endColor = lineRenderer.endColor;
            instanceLineRenderer.endWidth = lineRenderer.endWidth;
            instanceLineRenderer.forceRenderingOff = lineRenderer.forceRenderingOff;
            instanceLineRenderer.generateLightingData = lineRenderer.generateLightingData;
            instanceLineRenderer.hideFlags = lineRenderer.hideFlags;
            instanceLineRenderer.lightmapIndex = lineRenderer.lightmapIndex;
            instanceLineRenderer.lightmapScaleOffset = lineRenderer.lightmapScaleOffset;
            instanceLineRenderer.lightProbeProxyVolumeOverride = lineRenderer.lightProbeProxyVolumeOverride;
            instanceLineRenderer.lightProbeUsage = lineRenderer.lightProbeUsage;
            instanceLineRenderer.loop = lineRenderer.loop;
            instanceLineRenderer.material = lineRenderer.material;
            instanceLineRenderer.materials = lineRenderer.materials;
            instanceLineRenderer.motionVectorGenerationMode = lineRenderer.motionVectorGenerationMode;
            instanceLineRenderer.numCapVertices = lineRenderer.numCapVertices;
            instanceLineRenderer.numCornerVertices = lineRenderer.numCornerVertices;
            instanceLineRenderer.rayTracingMode = lineRenderer.rayTracingMode;
            instanceLineRenderer.realtimeLightmapIndex = lineRenderer.realtimeLightmapIndex;
            instanceLineRenderer.realtimeLightmapScaleOffset = lineRenderer.realtimeLightmapScaleOffset;
            instanceLineRenderer.receiveShadows = lineRenderer.receiveShadows;
            instanceLineRenderer.reflectionProbeUsage = lineRenderer.reflectionProbeUsage;
            instanceLineRenderer.rendererPriority = lineRenderer.rendererPriority;
            instanceLineRenderer.renderingLayerMask = lineRenderer.renderingLayerMask;
            instanceLineRenderer.shadowBias = lineRenderer.shadowBias;
            instanceLineRenderer.shadowCastingMode = lineRenderer.shadowCastingMode;
            instanceLineRenderer.sortingLayerID = lineRenderer.sortingLayerID;
            instanceLineRenderer.sortingLayerName = lineRenderer.sortingLayerName;
            instanceLineRenderer.sortingOrder = lineRenderer.sortingOrder;
            instanceLineRenderer.startColor = lineRenderer.startColor;
            instanceLineRenderer.startWidth = lineRenderer.startWidth;
            instanceLineRenderer.tag = lineRenderer.tag;
            instanceLineRenderer.textureMode = lineRenderer.textureMode;
            instanceLineRenderer.useWorldSpace = lineRenderer.useWorldSpace;
            instanceLineRenderer.widthCurve = lineRenderer.widthCurve;
            instanceLineRenderer.widthMultiplier = lineRenderer.widthMultiplier;
        }
    }
}