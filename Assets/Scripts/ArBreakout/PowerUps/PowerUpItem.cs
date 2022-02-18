using ArBreakout.Game;
using ArBreakout.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.PowerUps
{
    public class PowerUpItem : MonoBehaviour
    {
        [SerializeField] private Image _greyFill;
        [SerializeField] private Image _underlyingImage;

        private float _timeLeft;

        public void Init(Sprite image, float timeLeft)
        {
            _underlyingImage.sprite = image;
            _timeLeft = timeLeft;
        }

        private void Update()
        {
            if (_timeLeft > 0)
            {
                _greyFill.fillAmount = 1.0f - _timeLeft / PaddleBehaviour.PowerUpEffectDuration;
                _timeLeft -= GameTime.delta;
            }
        }
    }
}