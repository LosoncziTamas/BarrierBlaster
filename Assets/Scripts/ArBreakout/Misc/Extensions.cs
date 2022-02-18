using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Misc
{
    public static class Extensions
    {
        public static void SetVisibility(this CanvasGroup group, bool visible)
        {
            group.alpha = visible ? 1.0f : 0.0f;
            group.blocksRaycasts = visible;
        }

        public static Sequence AnimatePunchScale(this Transform transform, Vector3 targetScale, Ease ease,
            float duration)
        {
            return DOTween.Sequence()
                .Append(transform.DOScale(new Vector3(targetScale.x + 0.5f, targetScale.y + 0.5f, targetScale.z + 0.5f),
                        duration * 0.5f)
                    .SetEase(ease))
                .Append(transform.DOScale(targetScale, duration * 0.5f)
                    .SetEase(ease));
        }
    }
}