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
        
        [SerializeField] private MeshRenderer _sphereLeft;
        [SerializeField] private MeshRenderer _sphereRight;
        [SerializeField] private MeshRenderer _cylinder;

        [Header("Anim parameters")]
        [SerializeField] private float _cylinderSize;
        [SerializeField] private float _openAnimDuration;

        private Vector3 _velocity;
        private Vector3 _acceleration;
        private Vector3 _originalCylinderScale;
        private Vector3 _originalCylinderLocalPos;
        private PowerUpDescriptor _powerUpDescriptor;

        private bool _destroyed;

        private void Awake()
        {
            var cylinderTrans = _cylinder.transform;
            _originalCylinderScale = cylinderTrans.localScale;
            _originalCylinderLocalPos = cylinderTrans.localPosition;
            _sphereLeft.transform.localPosition = Vector3.zero;
            _sphereRight.transform.localPosition = Vector3.zero;
            cylinderTrans.localScale = Vector3.zero;
            cylinderTrans.localPosition = Vector3.zero;
            _gameEntities.Add(this);
        }

        public void Init(PowerUpDescriptor powerUpDescriptor)
        {
            PowerUp = powerUpDescriptor.powerUp;
            _text.text = powerUpDescriptor.letter;
            _powerUpDescriptor = powerUpDescriptor;
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
            DOTween.Sequence()
                .Insert(0, _sphereLeft.transform.DOLocalMoveX(-0.5f * _cylinderSize, _openAnimDuration))
                .Insert(0, _sphereLeft.material.DOColor(_powerUpDescriptor.outsideColor, _openAnimDuration))
                .Insert(0, _sphereRight.material.DOColor(_powerUpDescriptor.outsideColor, _openAnimDuration))
                .Insert(0, _sphereRight.transform.DOLocalMoveX(0.5f * _cylinderSize, _openAnimDuration))
                .Insert(0, _cylinder.transform.DOScale(_originalCylinderScale, _openAnimDuration))
                .Insert(0, _cylinder.transform.DOLocalMove(_originalCylinderLocalPos, _openAnimDuration))
                .Insert(0, _cylinder.material.DOColor(_powerUpDescriptor.insideColor, _openAnimDuration));
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