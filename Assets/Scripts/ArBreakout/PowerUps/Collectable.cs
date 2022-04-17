using ArBreakout.Common.Tween;
using ArBreakout.Game;
using ArBreakout.GamePhysics;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ArBreakout.PowerUps
{
    public class Collectable : MonoBehaviour
    {
        public const string GameObjectTag = "Collectable";

        public PowerUp PowerUp { get; private set; }

        [SerializeField] private MovementProperties _movementProperties;
        [SerializeField] private GameEntities _gameEntities;
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private ScaleProperties _scaleProperties;
        [SerializeField] private MeshRenderer _meshRenderer;
        
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
            _text.text = powerUpDescriptor.letter;
            _text.color = powerUpDescriptor.accentColor;
            
            var insideMat = powerUpDescriptor.insideMaterial;
            insideMat.color = Color.white;
            var outsideMat = powerUpDescriptor.outsideMaterial;
            outsideMat.color = powerUpDescriptor.accentColor;
            _meshRenderer.materials = new[] {outsideMat, insideMat};
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