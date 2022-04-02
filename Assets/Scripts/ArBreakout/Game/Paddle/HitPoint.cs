using System.Collections;
using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    public class HitPoint : MonoBehaviour
    {
        [SerializeField] private Color OnColor = new(1.0f, 0.0f, 0.0f, 1.0f);
        [SerializeField] private Color OffColor = new(0.0f, 0.0f, 0.0f, 1.0f);
        
        [SerializeField] private ChangeMeshColor _changeMeshColor;

        private bool _isOn;
        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (_isOn == value)
                {
                    return;
                }

                StartCoroutine(value ? ColorFade(OffColor, OnColor, 0.6f) : ColorFade(OnColor, OffColor, 0.6f));
                _isOn = value;
            }
        }
        
        private IEnumerator ColorFade(Color from, Color to, float duration)
        {
            var left = duration;
            while (left > 0)
            {
                left -= Time.deltaTime;
                var t = (duration - left) / duration;
                _changeMeshColor.SetColor(Color.Lerp(from, to, t));
                yield return new WaitForEndOfFrame();
            }
        }
    }
}