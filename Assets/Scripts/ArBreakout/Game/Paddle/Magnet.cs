using ArBreakout.Misc;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    public class Magnet : MonoBehaviour
    {
        [SerializeField] private Transform _leftTurret;
        [SerializeField] private Transform _rightTurret;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private MagnetProperties _magnetProperties;

        private float _activeTime;
        private float _textureOffset;
        private float _originalLocalY;
        private Gradient _originalGradient;
        private Sequence _activateSequence;
        private Sequence _deactivateSequence;
        
        private void Awake()
        {
            _originalGradient = _lineRenderer.colorGradient;
            _originalLocalY = transform.localPosition.y;
        }

        private Sequence CreateTurretReleaseAnimation()
        {
            return DOTween.Sequence()
                .Insert(0,
                    _leftTurret.DOLocalMoveY(_originalLocalY + _magnetProperties.OffsetY, _magnetProperties.AnimDuration)
                        .SetEase(_magnetProperties.Ease))
                .Insert(0,
                    _rightTurret.DOLocalMoveY(_originalLocalY + _magnetProperties.OffsetY, _magnetProperties.AnimDuration)
                        .SetEase(_magnetProperties.Ease))
                .OnComplete(() => _activateSequence = null);
        }

        private Sequence AnimateTurretWithdrawal()
        {
            return DOTween.Sequence()
                .Insert(0,
                    _leftTurret.DOLocalMoveY(_originalLocalY, _magnetProperties.AnimDuration)
                        .SetEase(_magnetProperties.Ease))
                .Insert(0,
                    _rightTurret.DOLocalMoveY(_originalLocalY, _magnetProperties.AnimDuration)
                        .SetEase(_magnetProperties.Ease))
                .OnComplete(() => _deactivateSequence = null);
        }
        
        public void Activate()
        {
            if (Mathf.Approximately(_activeTime, 0) || _activeTime < 0)
            {
                _activeTime = 0f;
                _textureOffset = 0f;
                CreateTurretReleaseAnimation();
            }
            _lineRenderer.enabled = true;
            _activeTime += _magnetProperties.Duration;
            _lineRenderer.colorGradient = _originalGradient;
        }

        public void Deactivate()
        {
            _activeTime = 0;
            _lineRenderer.enabled = false;
            _textureOffset = 0f;
            AnimateTurretWithdrawal();
        }
        
        private void Animate()
        {
            _textureOffset += Time.fixedDeltaTime * _magnetProperties.Speed;
            _lineRenderer.material.mainTextureOffset = new Vector2(_textureOffset, 0);
        }
        
        private void FixedUpdate()
        {
            if (_activeTime > 0f)
            {
                _activeTime -= GameTime.fixedDelta;
                Animate();
                if (_activeTime < 0.5f)
                {
                    var gradient = _lineRenderer.colorGradient;
                    var alphaKeys = gradient.alphaKeys;
                    alphaKeys[0] = new GradientAlphaKey(_activeTime, 0.0f);
                    gradient.SetKeys(
                        gradient.colorKeys,
                        alphaKeys
                    );
                    _lineRenderer.colorGradient = gradient;
                }
            }
            /*else if (_lineRenderer.enabled)
            {
                Deactivate();
            }*/
        }
    }
}