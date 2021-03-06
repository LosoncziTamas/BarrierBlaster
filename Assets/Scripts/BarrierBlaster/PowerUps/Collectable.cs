using BarrierBlaster.Common.Tween;
using BarrierBlaster.Game;
using BarrierBlaster.GamePhysics;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BarrierBlaster.PowerUps
{
    public class Collectable : MonoBehaviour
    {
        public const string GameObjectTag = "Collectable";

        public PowerUp PowerUp { get; private set; }

        [SerializeField] private MovementProperties _movementProperties;
        [SerializeField] private GameEntities _gameEntities;
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private TextMeshPro _textBack;
        [SerializeField] private ScaleProperties _scaleProperties;
        [SerializeField] private ChangeMeshColor _changeMeshColor;
        
        private Vector3 _velocity;
        private Vector3 _acceleration;
        private Vector3 _targetScale;

        private bool _destroyed;

        private void Awake()
        {
            _gameEntities.Add(this);
            transform.localScale = Vector3.zero;
        }

        public void Init(PowerUpDescriptor powerUpDescriptor)
        {
            PowerUp = powerUpDescriptor.powerUp;
            _textBack.text = _text.text = powerUpDescriptor.letter;
            _textBack.color = _text.color = powerUpDescriptor.accentColor;
            
            _changeMeshColor.SetColorForMaterial(Color.white, 1);
            _changeMeshColor.SetColorForMaterial(powerUpDescriptor.accentColor, 0);
        }

        private void FixedUpdate()
        {
            if (_destroyed)
            {
                return;
            }

            _acceleration = Vector3.back;
            _acceleration *= _movementProperties.speed;
            _acceleration += -_movementProperties.drag * _velocity;

            _velocity += BreakoutPhysics.CalculateVelocityDelta(_acceleration);
            transform.localPosition += BreakoutPhysics.CalculateMovementDelta(_acceleration, _velocity);
            transform.Rotate(Vector3.right, _movementProperties.rotation, Space.Self);
        }
        
        public void AnimateAppearance()
        {
            transform.DOScale(_scaleProperties.Scale, _scaleProperties.Duration).SetEase(_scaleProperties.Ease);
        }

        public void Destroy()
        {
            if (!_destroyed)
            {
                Destroy(gameObject);
            }

            _destroyed = true;
        }

        private void OnDestroy()
        {
            _gameEntities.Remove(this);
        }
    }
}