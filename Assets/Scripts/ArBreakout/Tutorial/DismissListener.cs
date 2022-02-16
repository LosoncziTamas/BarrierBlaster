using UnityEngine;
using UnityEngine.EventSystems;

namespace ArBreakout.Tutorial
{
    public class DismissListener : MonoBehaviour, IPointerDownHandler
    {   
        [SerializeField] private RectTransform _contentRect;
        [SerializeField] private TutorialOverlay _tutorial;
        
        // TODO: fix me
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(_contentRect, eventData.position))
            {
                _tutorial.DismissAndResume();
            }
        }
    }
}