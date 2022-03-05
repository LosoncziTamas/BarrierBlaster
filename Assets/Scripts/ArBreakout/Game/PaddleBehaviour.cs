using System;
using System.Collections.Generic;
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

        private static readonly int TotalPowerUpCount = Enum.GetNames(typeof(PowerUp)).Length;

        public const float PowerUpEffectDuration = 15.0f;
        
        private const float MaxWidthMultiplier = 2.0f;
        private const float MinWidthMultiplier = 0.5f;
        private const float DefaultSpeed = 18.0f;
        private const float MaxSpeed = 36.0f;
        private const float MinSpeed = 9.0f;
        private const float Drag = 2.0f;
        private const float WallCollisionBounce = 15.0f;
        private const float BallCollisionBounce = 5.0f;

        [SerializeField] private PlayerInput _playerInput;
        
        private readonly List<bool> _activePowerUps = new(TotalPowerUpCount);
        private readonly List<float> _activePowerUpTimes = new(TotalPowerUpCount);

        private Vector3 _localVelocity;
        private BallBehaviour _ballBehaviour;
        private Vector3 _parentStartPosition;
        private Vector3 _defaultScale;
        private Transform _parentTransform;
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

        public bool Magnetized => _activePowerUps[(int) PowerUp.Magnet];

        public class PowerUpState : EventArgs
        {
            public readonly List<PowerUp> ActivePowerUps;
            public readonly List<float> ActivePowerUpTimes;

            public PowerUpState(List<PowerUp> activePowerUps, List<float> activePowerUpTimes)
            {
                ActivePowerUps = activePowerUps;
                ActivePowerUpTimes = activePowerUpTimes;
            }
        }

        public static event EventHandler<PowerUpState> PowerUpStateChangeEvent;

        private void PublishPowerUpState()
        {
            var activePowerUps = new List<PowerUp>();
            var activePowerUpTimes = new List<float>();
            for (var i = 0; i < TotalPowerUpCount; i++)
            {
                if (_activePowerUps[i])
                {
                    activePowerUps.Add((PowerUp) i);
                    activePowerUpTimes.Add(_activePowerUpTimes[i]);
                }
            }

            PowerUpStateChangeEvent?.Invoke(this, new PowerUpState(activePowerUps, activePowerUpTimes));
        }

        private void Awake()
        {
            _gameEntities.Add(this);
            _defaultScale = transform.localScale;
            _parentTransform = transform.parent;
            _speed = DefaultSpeed;
            for (var i = 0; i < TotalPowerUpCount; i++)
            {
                _activePowerUps.Add(false);
                _activePowerUpTimes.Add(0.0f);
            }
        }

        private void Start()
        {
            AnchoredBallBehaviour = _parentTransform.GetComponentInChildren<BallBehaviour>();
        }
        
        public void ResetToDefaults()
        {
            _parentTransform.DOMove(_parentStartPosition, 0.6f);
            _speed = DefaultSpeed;
            AnimateScale(_defaultScale.x, _defaultScale.y, _defaultScale.z);

            for (var i = 0; i < TotalPowerUpCount; i++)
            {
                _activePowerUps[i] = false;
                _activePowerUpTimes[i] = 0.0f;
            }

            PublishPowerUpState();
        }

        private void AnimateScale(float x, float y, float z)
        {
            transform.DOScale(new Vector3(x, y, z), 0.6f);
        }

        private void FixedUpdate()
        {
            var localAcceleration = Vector3.zero;
            var controlSwitchIsActive = _activePowerUps[(int) PowerUp.ControlSwitch];

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
            
            UpdatePowerUpStates();
        }

        public void ActivatePowerUp(PowerUp powerUp)
        {
            var powerUpIdx = (int) powerUp;

            if (powerUp == PowerUp.Minifier)
            {
                const int magnifierIdx = (int) PowerUp.Magnifier;

                if (_activePowerUps[magnifierIdx])
                {
                    // Magnifiers and minifiers are complementary to each other.
                    _activePowerUps[powerUpIdx] = _activePowerUps[magnifierIdx] = false;
                    _activePowerUpTimes[powerUpIdx] = _activePowerUpTimes[magnifierIdx] = 0.0f;
                    AnimateScale(_defaultScale.x, _defaultScale.y, _defaultScale.z);
                }
                else
                {
                    _activePowerUpTimes[powerUpIdx] = PowerUpEffectDuration;
                    _activePowerUps[powerUpIdx] = true;
                    var newWidth = Mathf.Max(MinWidthMultiplier * _defaultScale.x,
                        transform.localScale.x * MinWidthMultiplier);
                    AnimateScale(newWidth, _defaultScale.y, _defaultScale.z);
                }
            }
            else if (powerUp == PowerUp.Magnifier)
            {
                const int minifierIdx = (int) PowerUp.Minifier;

                if (_activePowerUps[minifierIdx])
                {
                    _activePowerUps[powerUpIdx] = _activePowerUps[minifierIdx] = false;
                    _activePowerUpTimes[powerUpIdx] = _activePowerUpTimes[minifierIdx] = 0.0f;
                    AnimateScale(_defaultScale.x, _defaultScale.y, _defaultScale.z);
                }
                else
                {
                    _activePowerUpTimes[powerUpIdx] = PowerUpEffectDuration;
                    _activePowerUps[powerUpIdx] = true;

                    var localSclX = transform.localScale.x;
                    var newWidth = Mathf.Min(MaxWidthMultiplier * _defaultScale.x, localSclX * MaxWidthMultiplier);
                    var widthChanged = !Mathf.Approximately(newWidth, localSclX);

                    // Do not perform scaling and jumping when scale hasn't actually changed.
                    if (widthChanged)
                    {
                        AnimateScale(newWidth, _defaultScale.y, _defaultScale.z);

                        // Moving paddle away from the walls
                        var offsetX = newWidth * 0.25f;
                        var localPosX = _parentTransform.localPosition.x;
                        if (localPosX > 0)
                        {
                            _parentTransform.DOLocalMoveX(localPosX - offsetX, 0.6f);
                        }
                        else
                        {
                            _parentTransform.DOLocalMoveX(localPosX + offsetX, 0.6f);
                        }
                    }
                }
            }
            else if (powerUp == PowerUp.Accelerator)
            {
                const int deceleratorIdx = (int) PowerUp.Decelerator;

                if (_activePowerUps[deceleratorIdx])
                {
                    // Accelerators decelerators are complementary to each other.
                    _activePowerUps[powerUpIdx] = _activePowerUps[deceleratorIdx] = false;
                    _activePowerUpTimes[powerUpIdx] = _activePowerUpTimes[deceleratorIdx] = 0.0f;
                    _speed = DefaultSpeed;
                }
                else
                {
                    _activePowerUpTimes[powerUpIdx] = PowerUpEffectDuration;
                    _activePowerUps[powerUpIdx] = true;
                    _speed = MaxSpeed;
                }
            }
            else if (powerUp == PowerUp.Decelerator)
            {
                const int acceleratorIdx = (int) PowerUp.Accelerator;

                if (_activePowerUps[acceleratorIdx])
                {
                    _activePowerUps[powerUpIdx] = _activePowerUps[acceleratorIdx] = false;
                    _activePowerUpTimes[powerUpIdx] = _activePowerUpTimes[acceleratorIdx] = 0.0f;
                    _speed = DefaultSpeed;
                }
                else
                {
                    _activePowerUpTimes[powerUpIdx] = PowerUpEffectDuration;
                    _activePowerUps[powerUpIdx] = true;
                    _speed = MinSpeed;
                }
            }
            else if (powerUp == PowerUp.ControlSwitch || powerUp == PowerUp.Magnet)
            {
                _activePowerUpTimes[powerUpIdx] = PowerUpEffectDuration;
                _activePowerUps[powerUpIdx] = true;
            }

            PublishPowerUpState();
        }

        private void UpdatePowerUpStates()
        {
            for (var i = 0; i < TotalPowerUpCount; i++)
            {
                if (_activePowerUps[i])
                {
                    var timeLeft = _activePowerUpTimes[i];
                    if (Mathf.Approximately(timeLeft, 0.0f) || timeLeft < 0.0f)
                    {
                        _activePowerUpTimes[i] = 0.0f;
                        _activePowerUps[i] = false;

                        var powerUp = (PowerUp) i;
                        if (powerUp == PowerUp.Minifier || powerUp == PowerUp.Magnifier)
                        {
                            AnimateScale(_defaultScale.x, _defaultScale.y, _defaultScale.z);
                        }
                        else if (powerUp == PowerUp.Accelerator || powerUp == PowerUp.Decelerator)
                        {
                            _speed = DefaultSpeed;
                        }
                        else if (powerUp == PowerUp.Magnet && _ballBehaviour)
                        {
                            _ballBehaviour.Release(_localVelocity.magnitude);
                        }

                        PublishPowerUpState();
                    }
                    else
                    {
                        _activePowerUpTimes[i] -= GameTime.fixedDelta;
                    }
                }
            }
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
                ActivatePowerUp(collectable.PowerUp);
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