using System;
using System.Collections.Generic;
using ArBreakout.Game;
using ArBreakout.Misc;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.PowerUps
{
    public class PowerUpActivator : MonoBehaviour
    {
        
        public static event EventHandler<PowerUpState> PowerUpStateChangeEvent;
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
        
        public const float PowerUpEffectDuration = 15.0f;
        
        private static readonly int TotalPowerUpCount = Enum.GetNames(typeof(PowerUp)).Length;
        
        private const float MaxWidthMultiplier = 2.0f;
        private const float MinWidthMultiplier = 0.5f;
        private const float MaxSpeed = 36.0f;
        private const float MinSpeed = 9.0f;
                
        private readonly List<bool> _activePowerUps = new(TotalPowerUpCount);
        private readonly List<float> _activePowerUpTimes = new(TotalPowerUpCount);

        [SerializeField] private GameEntities _gameEntities;
        
        private Vector3 _defaultScale;
        
        public bool IsActive(PowerUp powerUp) => false;
        
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
            for (var i = 0; i < TotalPowerUpCount; i++)
            {
                _activePowerUps.Add(false);
                _activePowerUpTimes.Add(0.0f);
            }
        }

        public void ResetToDefaults()
        {
            for (var i = 0; i < TotalPowerUpCount; i++)
            {
                _activePowerUps[i] = false;
                _activePowerUpTimes[i] = 0.0f;
            }

            PublishPowerUpState();
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < TotalPowerUpCount; i++)
            {
                if (!_activePowerUps[i])
                {
                    continue;
                }
                
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
                        // _speed = DefaultSpeed;
                    }
                    else if (powerUp == PowerUp.Magnet) // && _ballBehaviour
                    {
                        // _ballBehaviour.Release(_localVelocity.magnitude);
                    }

                    PublishPowerUpState();
                }
                else
                {
                    _activePowerUpTimes[i] -= GameTime.fixedDelta;
                }
            }
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
                        /*var localPosX = _parentTransform.localPosition.x;
                        if (localPosX > 0)
                        {
                            _parentTransform.DOLocalMoveX(localPosX - offsetX, 0.6f);
                        }
                        else
                        {
                            _parentTransform.DOLocalMoveX(localPosX + offsetX, 0.6f);
                        }*/
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
                    // _speed = DefaultSpeed;
                }
                else
                {
                    _activePowerUpTimes[powerUpIdx] = PowerUpEffectDuration;
                    _activePowerUps[powerUpIdx] = true;
                    // _speed = MaxSpeed;
                }
            }
            else if (powerUp == PowerUp.Decelerator)
            {
                const int acceleratorIdx = (int) PowerUp.Accelerator;

                if (_activePowerUps[acceleratorIdx])
                {
                    _activePowerUps[powerUpIdx] = _activePowerUps[acceleratorIdx] = false;
                    _activePowerUpTimes[powerUpIdx] = _activePowerUpTimes[acceleratorIdx] = 0.0f;
                    // _speed = DefaultSpeed;
                }
                else
                {
                    _activePowerUpTimes[powerUpIdx] = PowerUpEffectDuration;
                    _activePowerUps[powerUpIdx] = true;
                    // _speed = MinSpeed;
                }
            }
            else if (powerUp == PowerUp.ControlSwitch || powerUp == PowerUp.Magnet)
            {
                _activePowerUpTimes[powerUpIdx] = PowerUpEffectDuration;
                _activePowerUps[powerUpIdx] = true;
            }

            PublishPowerUpState();
        }
        
        private void AnimateScale(float x, float y, float z)
        {
            transform.DOScale(new Vector3(x, y, z), 0.6f);
        }
    }
}