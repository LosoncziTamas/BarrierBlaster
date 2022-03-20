using System;
using System.Collections.Generic;
using System.Linq;
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
                        var ball = _gameEntities.Balls.First();
                        ball.ScaleDown();
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

        private void ScaleUpBall(int powerUpIdx)
        {
            const int minifierIdx = (int) PowerUp.Minifier;
            var ball = _gameEntities.Balls.First();
            
            // TODO: remove minifier
            if (_activePowerUps[minifierIdx])
            {
                _activePowerUps[powerUpIdx] = _activePowerUps[minifierIdx] = false;
                _activePowerUpTimes[powerUpIdx] = _activePowerUpTimes[minifierIdx] = 0.0f;
                ball.ScaleDown();
            }
            else
            {
                _activePowerUpTimes[powerUpIdx] = PowerUpEffectDuration;
                _activePowerUps[powerUpIdx] = true;
                ball.ScaleUp();
            }
        }

        public void ScaleDownBall(int powerUpIdx)
        {
            const int magnifierIdx = (int) PowerUp.Magnifier;
            var ball = _gameEntities.Balls.First();
            if (_activePowerUps[magnifierIdx])
            {
                // Magnifiers and minifiers are complementary to each other.
                _activePowerUps[powerUpIdx] = _activePowerUps[magnifierIdx] = false;
                _activePowerUpTimes[powerUpIdx] = _activePowerUpTimes[magnifierIdx] = 0.0f;
                ball.ScaleDown();
            }
            else
            {
                _activePowerUpTimes[powerUpIdx] = PowerUpEffectDuration;
                _activePowerUps[powerUpIdx] = true;
                ball.ScaleDown();
            }
        }

        public void DeActivatePowerUp(PowerUp powerUp)
        {
            var powerUpIdx = (int) powerUp;
            
            if (powerUp == PowerUp.Magnifier)
            {
                if (_activePowerUps[powerUpIdx])
                {
                    _activePowerUps[powerUpIdx] = _activePowerUps[powerUpIdx] = false;
                    _activePowerUpTimes[powerUpIdx] = _activePowerUpTimes[powerUpIdx] = 0.0f;
                    var ball = _gameEntities.Balls.First();
                    ball.ScaleDown();
                    PublishPowerUpState();
                }
            }
        }

        public void ActivatePowerUp(PowerUp powerUp)
        {
            var powerUpIdx = (int) powerUp;

            if (powerUp == PowerUp.Minifier)
            {
                ScaleDownBall(powerUpIdx);
            }
            else if (powerUp == PowerUp.Magnifier)
            {
                ScaleUpBall(powerUpIdx);
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
            else if (powerUp == PowerUp.Laser)
            {
                _gameEntities.Paddle.SetLaserBeamEnabled(true);
            }

            PublishPowerUpState();
        }
    }
}