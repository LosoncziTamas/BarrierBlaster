using ArBreakout.GameInput;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    public class Blaster : MonoBehaviour
    {
        [Header("Anim Parameters")] 
        [SerializeField] private float _duration;
        [SerializeField] private float _leftRotation;
        [SerializeField] private float _rightRotation;
        [SerializeField] private float _offset;
        [SerializeField] private Ease _ease;
        
        [SerializeField] private PlayerInput _gameInput;
        [SerializeField] private LaserBeam _leftBeam;
        [SerializeField] private LaserBeam _rightBeam;
        [SerializeField] private Transform _leftBlaster;
        [SerializeField] private Transform _rightBlaster;

        private bool _activated;
        private bool _shooting;
        private float _originalLocalY;
        private Vector3 _originalLeftRotation;
        private Vector3 _originalRightRotation;
        private Sequence _activateSequence;
        private Sequence _deactivateSequence;

        private void Awake()
        {
            _originalLocalY = transform.localPosition.y;
            _originalLeftRotation = _leftBlaster.localRotation.eulerAngles;
            _originalRightRotation = _rightBlaster.localRotation.eulerAngles;
        }
        
        private Sequence CreateAnimSequence()
        {
            return DOTween.Sequence()
                .Append(transform.DOLocalMoveY(_originalLocalY + _offset, _duration).SetEase(_ease))
                .Insert(_duration * 0.5f,
                    _rightBlaster
                        .DOLocalRotate(_originalRightRotation + Vector3.forward * _rightRotation, _duration * 0.5f)
                        .SetEase(_ease))
                .Insert(_duration * 0.5f,
                    _leftBlaster
                        .DOLocalRotate(_originalLeftRotation + Vector3.forward * _leftRotation, _duration * 0.5f)
                        .SetEase(_ease))
                .OnComplete(() => _activateSequence = null);
        }

        private Sequence CreateDeactivateAnimSequence()
        {
            return DOTween.Sequence()
                .Insert(0,
                    _rightBlaster
                        .DOLocalRotate(_originalRightRotation, _duration * 0.5f)
                        .SetEase(_ease))
                .Insert(0,
                    _leftBlaster
                        .DOLocalRotate(_originalLeftRotation, _duration * 0.5f)
                        .SetEase(_ease))
                .Insert(_duration * 0.5f, transform.DOLocalMoveY(_originalLocalY, _duration).SetEase(_ease))
                .OnComplete(() =>
                {
                    _deactivateSequence = null;
                });
        }
        
        public void Activate()
        {
            _activated = true;
            if (_activateSequence != null && _activateSequence.IsPlaying())
            {
                return;
            }
            
            gameObject.SetActive(true);
            _activateSequence = CreateAnimSequence();
        }

        public void Deactivate()
        {
            _leftBeam.EndLaunching();
            _rightBeam.EndLaunching();
            _shooting = false;
            _activated = false;
            if (_deactivateSequence != null && _deactivateSequence.IsPlaying())
            {
                return;
            }

            _deactivateSequence = CreateDeactivateAnimSequence();
        }

        private void Update()
        {
            if (!_activated)
            {
                return;
            }
            
            if (!_shooting && _gameInput.Fire)
            {
                _leftBeam.BeginLaunching();
                _rightBeam.BeginLaunching();
                _shooting = true;
            }
            
            if (_shooting && (!_leftBeam.Launching || !_rightBeam.Launching))
            {
                Deactivate();
            }
        }
    }
}