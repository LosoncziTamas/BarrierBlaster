using UnityEngine;
using UnityEngine.EventSystems;

namespace ArBreakout.GameInput
{
    public class PointerDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public bool PointerDown => _pointerDown;
        
        private bool _pointerDown;

        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pointerDown = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _pointerDown = false;
        }
    }
}