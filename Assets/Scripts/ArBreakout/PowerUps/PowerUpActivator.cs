using System;
using System.Collections.Generic;
using System.Linq;
using ArBreakout.Common;
using ArBreakout.Common.Variables;
using ArBreakout.Game;
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
        
        private readonly List<bool> _activePowerUps = new(TotalPowerUpCount);
        private readonly List<float> _activePowerUpTimes = new(TotalPowerUpCount);

        [SerializeField] private GameEntities _gameEntities;
        [SerializeField] private BallBehaviour _ballPrefab;
        [SerializeField] private FloatVariable _magnetActiveTime;

        private Vector3 _defaultScale;

        public bool IsActive(PowerUp powerUp)
        {
            var idx = (int)powerUp;
            return idx < _activePowerUps.Count && _activePowerUps[idx];
        }
        
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
                var powerUp = (PowerUp) i;
                if (Mathf.Approximately(timeLeft, 0.0f) || timeLeft < 0.0f)
                {
                    DeActivatePowerUp(powerUp);
                }
                else
                {
                    _activePowerUpTimes[i] -= GameTime.FixedDelta;
                    if (powerUp == PowerUp.Magnet)
                    {
                        _magnetActiveTime.Value = _activePowerUpTimes[i];
                    }
                }
            }
        }

        private void ScaleUpBall()
        {
            UIMessageController.Instance.DisplayMessage("amplified", 1.0f, 0);
            var powerUpIdx = (int) PowerUp.Magnifier;
            var balls = _gameEntities.Balls;
            _activePowerUpTimes[powerUpIdx] = PowerUpEffectDuration;
            _activePowerUps[powerUpIdx] = true;
            foreach (var ball in balls)
            {
                ball.ScaleUp();
            }
        }

        private void MagnetizePaddle()
        {
            UIMessageController.Instance.DisplayMessage("magnetized", 1.0f, 0);
            const int idx = (int) PowerUp.Magnet;
            _gameEntities.Paddle.SetMagnetEnabled(true);
            _magnetActiveTime.Value = PowerUpEffectDuration;
            _activePowerUpTimes[idx] = PowerUpEffectDuration;
            _activePowerUps[idx] = true;
        }

        public void DeActivatePowerUp(PowerUp powerUp)
        {
            var powerUpIdx = (int) powerUp;
            var isActive = _activePowerUps[powerUpIdx];
            if (!isActive)
            {
                return;
            }
            
            if (powerUp == PowerUp.Magnifier)
            {
                var balls = _gameEntities.Balls;
                foreach (var ball in balls)
                {
                    ball.ScaleDown();
                }
            }
            else if (powerUp == PowerUp.Magnet)
            {
                _magnetActiveTime.Value = 0;
                _gameEntities.Paddle.SetMagnetEnabled(false);
            }
            
            _activePowerUps[powerUpIdx] = _activePowerUps[powerUpIdx] = false;
            _activePowerUpTimes[powerUpIdx] = _activePowerUpTimes[powerUpIdx] = 0.0f;
            PublishPowerUpState();
        }

        private void OnGUI()
        {
            GUILayout.Space(20);
            if (GUILayout.Button("Magnetize"))
            {
                MagnetizePaddle();
            }
            if (GUILayout.Button("Spawn"))
            {
                SpawnBall();
            }
            if (GUILayout.Button("Scale"))
            {
                ScaleUpBall();
            }
        }

        private void SpawnBall()
        {
            UIMessageController.Instance.DisplayMessage("extra ball", 1.0f, 0);
            var ball = _gameEntities.Balls.First();
            var ballTrans = ball.transform;
            var spawnedBall = Instantiate(_ballPrefab, ballTrans.parent);
            
            spawnedBall.transform.position = ballTrans.position;
            var newDir = Vector3.Scale(Mathf.Approximately(0, ball.LocalVelocity.sqrMagnitude) ? Vector3.forward : ball.LocalVelocity, new Vector3(-1.0f, 1.0f, 1.0f));
            spawnedBall.Release(0, newDir.normalized);
        }

        private void ActivateLaserBeam()
        {
            UIMessageController.Instance.DisplayMessage("laser beam", 1.0f, 0);
            _gameEntities.Paddle.SetLaserBeamEnabled(true);
        }

        public void ActivatePowerUp(PowerUp powerUp)
        {
            var powerUpIdx = (int) powerUp;
            
            switch (powerUp)
            {
                case PowerUp.Magnifier:
                    ScaleUpBall();
                    break;
                case PowerUp.Laser:
                    ActivateLaserBeam();
                    break;
                case PowerUp.BallSpawner:
                    SpawnBall();
                    break;
                case PowerUp.Magnet:
                    MagnetizePaddle();
                    break;
                case PowerUp.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(powerUp), powerUp, null);
            }
            
            PublishPowerUpState();
        }
    }
}