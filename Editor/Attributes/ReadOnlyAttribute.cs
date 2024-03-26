using System;
using UnityEngine;

namespace IA.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public Color InspectorColor { get; set; }

        public ReadOnlyAttribute()
        {
            InspectorColor = default;
        }

        public ReadOnlyAttribute(float r = 1f, float g = 1f, float b = 1f, float a = 1f)
        {
            InspectorColor = new Color(r, g, b, a);
        }
    }
}
