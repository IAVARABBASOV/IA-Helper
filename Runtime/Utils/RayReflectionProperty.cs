using UnityEngine;
using System;

namespace IA.Utils
{
    [System.Serializable]
    public struct RayReflectionProperty
    {
        public Color HitColor;
        public float RayDistance;
        public LayerMask HitMask;
        public Func<Vector3, Vector3> ReflectDirectionReturnHandler;

        public RayReflectionProperty(
            Color hitColor = default,
            float distance = Mathf.Infinity,
            LayerMask hitMask = default,
            Func<Vector3, Vector3> reflectDirectionResultHandler = null)
        {
            HitColor = hitColor;
            RayDistance = distance;
            HitMask = hitMask;
            ReflectDirectionReturnHandler = reflectDirectionResultHandler;
        }
    }
}