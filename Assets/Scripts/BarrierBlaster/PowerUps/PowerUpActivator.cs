using System;
using System.Collections.Generic;
using System.Linq;
using BarrierBlaster.Common;
using BarrierBlaster.Game;
using BarrierBlaster.Game.Ball;
using UnityEngine;

namespace BarrierBlaster.PowerUps
{
    public partial class PowerUpActivator : MonoBehaviour
    {
        public const float PowerUpEffectDuration = 15.0f;
        
        private static readonly int TotalPowerUpCount = Enum.GetNames(typeof(PowerUp)).Length;
        
        private readonly List<bool> _activePowerUps = new(TotalPowerUpCount);
        private readonly List<float> _activePowerUpTimes = new(TotalPowerUpCount);

        [SerializeField] private GameEntities _gameEntities;
        [SerializeField] private BallBehaviour _ballPrefab;

        private Vector3 _defaultScale;

        public bool IsActive(PowerUp powerUp)
        {
            var idx = (int)powerUp;
            Debug.Assert(idx < _activePowerUps.Count);
            return _activePowerUps[idx];
        }

        public float GetActiveTimeLeft(PowerUp powerUp)
        {
            var idx = (int)powerUp;
            Debug.Assert(idx < _activePowerUpTimes.Count);
            var result = _activePowerUpTimes[idx];
            if (result > 0.0f)
            {
                Debug.Assert(IsActive(powerUp));
            }
            return result;
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
            foreach (PowerUp powerUp in Enum.GetValues(typeof(PowerUp)))
            {
                DeActivatePowerUp(powerUp);
            }
        }

        private static bool TimeUp(float time) => Mathf.Approximately(time, 0.0f) || time < 0.0f;

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
                if (TimeUp(timeLeft))
                {
                    DeActivatePowerUp(powerUp);
                }
                else
                {
                    _activePowerUpTimes[i] -= GameTime.FixedDelta;
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
                _gameEntities.Paddle.SetMagnetEnabled(false);
            }
            else if (powerUp == PowerUp.Laser)
            {
                _gameEntities.Paddle.SetLaserBeamEnabled(false);
            }
            
            _activePowerUps[powerUpIdx] = _activePowerUps[powerUpIdx] = false;
            _activePowerUpTimes[powerUpIdx] = _activePowerUpTimes[powerUpIdx] = 0.0f;
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
            const int idx = (int) PowerUp.Laser;
            _activePowerUpTimes[idx] = PowerUpEffectDuration;
            _activePowerUps[idx] = true;
        }

        public void ActivatePowerUp(PowerUp powerUp)
        {
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Pop);
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
        }
    }
}