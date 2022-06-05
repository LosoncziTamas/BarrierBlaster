using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BarrierBlaster.Common.Tween
{
    public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ScaleProperties _hoverProperties;
     
        private DG.Tweening.Tween _hoverAnimTween;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_hoverAnimTween == null)
            {
                _hoverAnimTween = transform
                    .DOScale(_hoverProperties.Scale, _hoverProperties.Duration)
                    .SetEase(_hoverProperties.Ease)
                    .SetAutoKill(false);
            }
            else
            {
                _hoverAnimTween.Restart();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hoverAnimTween.PlayBackwards();
        }
    }
}