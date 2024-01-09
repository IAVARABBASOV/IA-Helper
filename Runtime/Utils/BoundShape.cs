using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IA.Utils
{
    [ExecuteAlways]
    public class BoundShape : MonoBehaviour
    {
        [SerializeField] private bool dontDestroyBoundsOnDestroy = false;
        [Space(5f)]
        [SerializeField] private Transform boundsParent;

        [Space(20f)]
        [SerializeField] private Transform boundsOrigin = null;

        [Header("Corner Properties of the Bounds")]
        [SerializeField] private List<Transform> corners = new List<Transform>();

        [SerializeField] private Bounds objectBounds = new Bounds();
        public Bounds GetBounds => objectBounds;

        [Space(5f)]
        [SerializeField, Range(0f, 1f)] private float boundsTransparency = 0.25f;

        private Vector3 lastPosition;
        private Quaternion lastRotation;

#if UNITY_EDITOR

        private void Start()
        {
            TryCreateBoundsTransformsInChild();
        }

        private void OnDestroy()
        {
            if (dontDestroyBoundsOnDestroy) return;

          //  Debug.LogError("Bounds Destroyed");

            /// Destroy Bounds
            DestroyImmediate(boundsParent.gameObject);
        }

        private void TryCreateBoundsTransformsInChild()
        {
            /// Create Parent of the Bounds
            if (boundsParent == null) boundsParent = UnityExtensions.CreateTransform($"Bounds of {name}", Vector3.zero, Quaternion.identity, Vector3.one, transform);

            /// Create bounds Center
            if (boundsOrigin == null) boundsOrigin = UnityExtensions.CreateTransform("Center", Vector3.zero, Quaternion.identity, Vector3.one, boundsParent);

            /// Create Corner of the Bounds
            if (corners.Count == 0)
            {
                /// Default Corners Count
                int maxCornerCount = 4;

                /// Generate Corners in Loop
                for (int i = 0; i < maxCornerCount; i++)
                {
                    Vector3 cornerPosition = GetCornerPosition(i);
                    Transform corner = UnityExtensions.CreateTransform($"Corner {i}", cornerPosition, Quaternion.identity, Vector3.one, boundsParent);
                    corners.Add(corner);
                }
            }
        }

        private Vector3 GetCornerPosition(int i)
        {
            float x = (i % 2 == 0) ? -0.5f : 0.5f;
            float y = (i < 2) ? 0.5f : -0.5f;
            return new Vector3(x, y, 0f);
        }

#endif

        private void Update()
        {
            /// Calculate bounds for each rotate
            if (lastPosition != transform.position || lastRotation != transform.rotation)
            {
                lastPosition = transform.position;
                lastRotation = transform.rotation;

                CalculateBoundsShape();
            }
        }

        private void CalculateBoundsShape()
        {
            /// Get the current position, rotation and scale of the object
            Vector3 currentPosition = boundsOrigin.position;
            Quaternion currentRotation = (boundsOrigin.rotation);
            Vector3 currentSize = boundsOrigin.localScale;

            /// Update the local bounds based on the position, rotation and size
            objectBounds.center = currentPosition;
            objectBounds.size = currentRotation * (currentSize);

            /// Calculate corner positions and keep bounds size inside of Corner
            foreach (Transform corner in corners)
            {
                objectBounds.Encapsulate(corner.position);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Color boundsColor = Color.yellow;
            boundsColor.a = boundsTransparency;
            Gizmos.color = boundsColor;

            Gizmos.DrawCube(objectBounds.center, objectBounds.size);
        }
#endif
    }
}
