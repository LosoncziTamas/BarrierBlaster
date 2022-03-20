using ArBreakout.Game.Course;
using ArBreakout.Game.Paddle;
using ArBreakout.GameInput;
using ArBreakout.GamePhysics;
using ArBreakout.Misc;
using ArBreakout.PowerUps;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

namespace ArBreakout.Game
{
    [RequireComponent(typeof(Collider))]
    public class PaddleBehaviour : MonoBehaviour
    {
        public const string GameObjectTag = "Paddle";
        
        private const float DefaultSpeed = 18.0f;
        private const float Drag = 2.0f;
        private const float WallCollisionBounce = 15.0f;
        private const float BallCollisionBounce = 5.0f;

        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private PaddleHitPoints _hitPoints;
        [SerializeField] private LaserBeam _laserBeam;
        
        private Vector3 _localVelocity;
        private BallBehaviour _ballBehaviour;
        private Vector3 _parentStartPosition;
        private Vector3 _defaultScale;
        private Transform _parentTransform;
        private PowerUpActivator _powerUpActivator;
        private float _speed;
        
        [SerializeField] private GameEntities _gameEntities;
        
        private void OnDestroy()
        {
            _gameEntities.Remove(this);
        }

        public void StoreCurrentPositionAsStartPosition()
        {
            _parentStartPosition = transform.parent.position;
        }

        public BallBehaviour AnchoredBallBehaviour
        {
            set => _ballBehaviour = value;
        }

        public bool Magnetized => _powerUpActivator.IsActive(PowerUp.Magnet);

        private void Awake()
        {
            _gameEntities.Add(this);
            var transform1 = transform;
            _defaultScale = transform1.localScale;
            _parentTransform = transform1.parent;
            _speed = DefaultSpeed;
            _powerUpActivator = FindObjectOfType<PowerUpActivator>();
        }

        private void Start()
        {
            AnchoredBallBehaviour = _parentTransform.GetComponentInChildren<BallBehaviour>();
        }
        
        public void ResetToDefaults()
        {
            _hitPoints.ResetToFull();
            _powerUpActivator.ResetToDefaults();
            _parentTransform.DOMove(_parentStartPosition, 0.6f);
            transform.DOScale(_defaultScale, 0.6f);
            _speed = DefaultSpeed;
            SetLaserBeamEnabled(false);
        }

        public void SetLaserBeamEnabled(bool laserBeamEnabled)
        {
            if (laserBeamEnabled)
            {
                _laserBeam.BeginLaunching();
            }
            else
            {
                _laserBeam.EndLaunching();
            }
        }

        private void FixedUpdate()
        {
            var localAcceleration = Vector3.zero;
            var controlSwitchIsActive = _powerUpActivator.IsActive(PowerUp.ControlSwitch);

            if (_playerInput.Left)
            {
                localAcceleration.x += controlSwitchIsActive ? 1.0f : -1.0f;
            }

            if (_playerInput.Right)
            {
                localAcceleration.x += controlSwitchIsActive ? -1.0f : 1.0f;
            }

            if (_playerInput.Fire && _ballBehaviour)
            {
                _ballBehaviour.Release(_localVelocity.magnitude);
                _ballBehaviour = null;
            }

            localAcceleration *= _speed;
            localAcceleration += -Drag * _localVelocity;

            _localVelocity += BreakoutPhysics.CalculateVelocityDelta(localAcceleration);
            // We move the parent transform instead of the paddle. This is a workaround used to avoid unwanted scale of the ball.
            transform.parent.localPosition += BreakoutPhysics.CalculateMovementDelta(localAcceleration, _localVelocity);
        }

        private void OnCollisionEnter(Collision other)
        {
            // Debug.DrawRay(other.contacts[0].point, other.contacts[0].normal * other.contacts[0].separation, Color.green, 2, false);

            if (other.gameObject.CompareTag(WallBehaviour.GameObjectTag))
            {
                var contact = BreakoutPhysics.ExtractContactPoint(other);
                var reflectionGlobal = Vector3.Reflect(transform.TransformVector(_localVelocity), contact.Normal) *
                                       contact.Separation;
                // Change the velocity so it is properly bounced back from the wall.
                _localVelocity = transform.InverseTransformVector(reflectionGlobal) * WallCollisionBounce;
            }
            else if (other.gameObject.CompareTag(BallBehaviour.GameObjectTag))
            {
                var contact = BreakoutPhysics.ExtractContactPoint(other);
                var reflectionGlobal = Vector3.Reflect(transform.TransformVector(_localVelocity), contact.Normal) *
                                       contact.Separation;
                // Apply some bounce effect to the paddle in order to avoid tunnelling when the ball is moving between the wall and the paddle.
                _localVelocity += transform.InverseTransformVector(reflectionGlobal) * BallCollisionBounce;
                // Only apply the reflection force in one dimension.
                _localVelocity.Scale(Vector3.right);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Collectable.GameObjectTag))
            {
                var collectable = other.gameObject.GetComponentInParent<Collectable>();
                _powerUpActivator.ActivatePowerUp(collectable.PowerUp);
                collectable.Destroy();
            }
        }

        private void OnCollisionStay(Collision collisionInfo)
        {
            Debug.DrawRay(collisionInfo.contacts[0].point,
                collisionInfo.contacts[0].normal * collisionInfo.contacts[0].separation, Color.red, 2, false);

            if (collisionInfo.gameObject.CompareTag(WallBehaviour.GameObjectTag) ||
                collisionInfo.gameObject.CompareTag(BallBehaviour.GameObjectTag))
            {
                var contact = BreakoutPhysics.ExtractContactPoint(collisionInfo);
                var localContactNormal = transform.InverseTransformDirection(contact.Normal);
                var correction = localContactNormal * contact.Separation;

                // Move paddle object away from the other (horizontally).
                correction.Scale(Vector3.right);
                _parentTransform.localPosition += correction;

                // Making sure the velocity is also changed, no just the position. This can happen if OnCollisionEnter is skipped and OnCollisionStay is called.
                var newVelocity = _localVelocity.magnitude * localContactNormal;
                if (collisionInfo.gameObject.CompareTag(WallBehaviour.GameObjectTag))
                {
                    // Apply bounce effect for walls
                    newVelocity += localContactNormal * WallCollisionBounce;
                }

                newVelocity.Scale(Vector3.right);
                _localVelocity = newVelocity;
            }
        }

        [UsedImplicitly]
        public void ReAnchorBall()
        {
            // Assuming there is only one ball
            var ball = _gameEntities.Balls[0];
            GamePlayUtils.AnchorBallToPaddle(ball, this);
        }
    }
}