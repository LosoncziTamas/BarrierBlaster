using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ArBreakout.GameInput
{
    public class PointerDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private const float HighlightDuration = 0.7f;
        public bool PointerDown { get; private set; }
        
        [SerializeField] private Image _image;

        private Color _defaultColor;
        private float _accumulator;
        
        private void Awake()
        {
            _defaultColor = _image.color;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerDown = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerDown = false;
        }

        public void Highlight()
        {
            var c = _image.color;
            c.a = 0.4f;
            _image.color = c;
            _accumulator = HighlightDuration;
        }
        
        private void Update()
        {
            var diff = _image.color.a - _defaultColor.a;
            if (Mathf.Approximately(diff, 0))
            {
                _accumulator = 0.0f;
                return;
            }
            _accumulator -= Time.deltaTime;
            _image.color = Color.Lerp(_image.color, _defaultColor, (HighlightDuration - _accumulator));
        }
    }
}