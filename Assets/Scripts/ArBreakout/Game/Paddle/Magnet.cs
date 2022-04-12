using ArBreakout.Common;
using ArBreakout.Common.Variables;
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
        [SerializeField] private FloatVariable _magnetActiveTime;

        private float _textureOffset;
        private float _originalLocalY;
        private Gradient _originalGradient;
        
        private void Awake()
        {
            _originalGradient = _lineRenderer.colorGradient;
            _originalLocalY = _leftTurret.localPosition.y;
        }

        private void CreateTurretReleaseAnimation()
        {
            DOTween.Sequence()
                .Insert(0,
                    _leftTurret.DOLocalMoveY(_originalLocalY + _magnetProperties.OffsetY, _magnetProperties.AnimDuration)
                        .SetEase(_magnetProperties.Ease))
                .Insert(0,
                    _rightTurret.DOLocalMoveY(_originalLocalY + _magnetProperties.OffsetY, _magnetProperties.AnimDuration)
                        .SetEase(_magnetProperties.Ease))
                .OnComplete(() => _lineRenderer.enabled = true);
        }

        private void AnimateTurretWithdrawal()
        {
            DOTween.Sequence()
                .Insert(0,
                    _leftTurret.DOLocalMoveY(_originalLocalY, _magnetProperties.AnimDuration)
                        .SetEase(_magnetProperties.Ease))
                .Insert(0,
                    _rightTurret.DOLocalMoveY(_originalLocalY, _magnetProperties.AnimDuration)
                        .SetEase(_magnetProperties.Ease));
        }
        
        public void Activate()
        {
            _textureOffset = 0f;
            CreateTurretReleaseAnimation();
            _lineRenderer.colorGradient = _originalGradient;
        }

        public void Deactivate()
        {
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
            var activeTime = _magnetActiveTime.Value;
            if (activeTime > 0f)
            {
                Animate();
                if (activeTime < 0.5f)
                {
                    var gradient = _lineRenderer.colorGradient;
                    var alphaKeys = gradient.alphaKeys;
                    alphaKeys[0] = new GradientAlphaKey(activeTime, 0.0f);
                    gradient.SetKeys(
                        gradient.colorKeys,
                        alphaKeys
                    );
                    _lineRenderer.colorGradient = gradient;
                }
            }
        }
    }
}