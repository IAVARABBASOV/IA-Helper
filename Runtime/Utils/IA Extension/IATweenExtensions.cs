using TMPro;
using UnityEngine;
using UnityEngine.Events;

#if DGTweening
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

        public static Tween DoRotate(this Transform _transform, Vector3 _startValue, Vector3 _addValue, float _rotDuration, RotateMode _rotMode, Ease _easeType, UnityAction _onCompleted = null)
        {
            _transform.localEulerAngles = _startValue;

            Vector3 endValue = _transform.localEulerAngles + _addValue;

            Tween tween = _transform.
                DOLocalRotate(endValue, _rotDuration, _rotMode).
                SetEase(_easeType).
                OnComplete(() => _onCompleted?.Invoke());

            return tween;
        }
    }
}
#endif