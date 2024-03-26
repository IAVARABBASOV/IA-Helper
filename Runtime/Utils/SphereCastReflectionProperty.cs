using UnityEngine;

namespace IA.Utils
{
    [System.Serializable]
    public struct SphereCastReflectionProperty
    {
        public float Radius;
        public float Distance;
        public LayerMask HitMask;
        public Color HitColor;
        public Color LineColor;

        public SphereCastReflectionProperty(float _radius = 1, float _distance = 10f, LayerMask _hitMask = default,
             Color _hitColor = default, Color _lineColor = default)
        {
            Radius = _radius;
            Distance = _distance;
            HitMask = _hitMask;
            HitColor = _hitColor;
            LineColor = _lineColor;
        }
    }
}