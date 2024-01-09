using TMPro;
using DG.Tweening;

namespace IA.Utils
{
    public static class IATweenExtensions
    {
        public static Tween VisibleCharactersAnimation(this TextMeshProUGUI tmp_Target, int endValue, float duration)
        {
            Tween tween = DOTween.To(
                getter: () => tmp_Target.maxVisibleCharacters,
                setter: (x) => tmp_Target.maxVisibleCharacters = x, endValue, duration);

            return tween;
        }
    }
}
