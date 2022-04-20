using System;
using System.Collections.Generic;
using System.Linq;
using ArBreakout.Common;
using ArBreakout.Common.Variables;
using ArBreakout.Game;
using UnityEngine;

namespace ArBreakout.PowerUps
{
    public partial class PowerUpActivator : MonoBehaviour
    {
        public const float PowerUpEffectDuration = 15.0f;
        
        [SerializeField] private GameEntities _gameEntities;
        [SerializeField] private BallBehaviour _ballPrefab;
        [SerializeField] private FloatVariable _magnetActiveTime;
        [SerializeField] private FloatVariable _laserBeamActiveTime;

        private Vector3 _defaultScale;

        public bool IsActive(PowerUp powerUp)
        {
            switch (powerUp)
            {
                case PowerUp.Magnet:
                    return _magnetActiveTime.Value > 0;
                case PowerUp.Laser:
                    return _laserBeamActiveTime.Value > 0;
                case PowerUp.BallSpawner:
                case PowerUp.None:
                case PowerUp.Magnifier:
                default:
                    return false;
            }
        }
        
        private void Awake()
        {
            _magnetActiveTime.Value = 0;
            _laserBeamActiveTime.Value = 0;
        }

        public void ResetToDefaults()
        {
            foreach (PowerUp powerUp in Enum.GetValues(typeof(PowerUp)))
            {
                DeActivatePowerUp(powerUp);
            }
        }

        private bool IsZeroOrLess(float n)
        {
            return Mathf.Approximately(n, 0.0f) || n < 0.0f;
        }

        private void FixedUpdate()
        {
            // TODO: clean this up
            /*
            if (!IsZeroOrLess(_magnetActiveTime.Value))
            {
                _magnetActiveTime.Value -= GameTime.FixedDelta;
            }
            else
            {
                DeActivatePowerUp(PowerUp.Magnet);
            }
            
            if (!IsZeroOrLess(_laserBeamActiveTime.Value))
            {
                _laserBeamActiveTime.Value -= GameTime.FixedDelta;
            }
            else
            {
                DeActivatePowerUp(PowerUp.Laser);
            }
            */
        }

        private void ScaleUpBall()
        {
            UIMessageController.Instance.DisplayMessage("amplified", 1.0f, 0);
            var balls = _gameEntities.Balls;
            foreach (var ball in balls)
            {
                ball.ScaleUp();
            }
        }



        public void DeActivatePowerUp(PowerUp powerUp)
        {
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
            else if (powerUp == PowerUp.Laser)
            {
                _laserBeamActiveTime.Value = 0;
                _gameEntities.Paddle.SetLaserBeamEnabled(false);
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
        
        private void MagnetizePaddle()
        {
            UIMessageController.Instance.DisplayMessage("magnetized", 1.0f, 0);
            const int idx = (int) PowerUp.Magnet;
            _gameEntities.Paddle.SetMagnetEnabled(true);
            _magnetActiveTime.Value = PowerUpEffectDuration;
        }

        private void ActivateLaserBeam()
        {
            UIMessageController.Instance.DisplayMessage("laser beam", 1.0f, 0);
            _gameEntities.Paddle.SetLaserBeamEnabled(true);
        }

        public void ActivatePowerUp(PowerUp powerUp)
        {
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