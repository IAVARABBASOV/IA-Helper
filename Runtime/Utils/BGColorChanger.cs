using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace IA.Utils
{
    [ExecuteAlways]
    public class BGColorChanger : Singleton<BGColorChanger>
    {
        [System.Serializable]
        public class ColorValues
        {
            [SerializeField] private string name;
            public string GetName => name;
            [Range(0f, 1f), Space] public float HUE = .1f;
            [Range(0f, 1f), Space] public float Saturation = .5f;
            [Range(0f, 1f), Space] public float Brightness = 1f;

            public Color GetColor() => Color.HSVToRGB(HUE, Saturation, Brightness);
        }

        [System.Serializable]
        public class GradiendtColorValues
        {
            public string Name = "Gradient";
            public ColorValues TopColor = new ColorValues();
            public ColorValues MiddleColor = new ColorValues();
            public ColorValues BottomColor = new ColorValues();
        }

        [SerializeField] private int gradientIndex = 0;

        public List<GradiendtColorValues> Gradiendts = new List<GradiendtColorValues>();

        [SerializeField] private Material mat = null;

        private void Update()
        {
            if (mat)
            {
                if (Gradiendts.Count > 0)
                {
                    gradientIndex =
                        gradientIndex > Gradiendts.Count - 1 ?
                        Gradiendts.Count - 1 :
                        gradientIndex < 0 ? 0 : gradientIndex;

                    mat.SetColor(
                        Gradiendts[gradientIndex].TopColor.GetName,
                        Gradiendts[gradientIndex].TopColor.GetColor());

                    mat.SetColor(
                        Gradiendts[gradientIndex].MiddleColor.GetName,
                        Gradiendts[gradientIndex].MiddleColor.GetColor());

                    mat.SetColor(
                        Gradiendts[gradientIndex].BottomColor.GetName,
                        Gradiendts[gradientIndex].BottomColor.GetColor());
                }
            }
        }

        public void RandomGradient()
        {
            if (Gradiendts.Count > 0)
            {
                gradientIndex = Random.Range(0, Gradiendts.Count);
            }
        }
    }
}
