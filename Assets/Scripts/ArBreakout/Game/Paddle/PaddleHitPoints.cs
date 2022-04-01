using ArBreakout.Misc;
using JetBrains.Annotations;
using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    public class PaddleHitPoints : MonoBehaviour
    {
        [SerializeField] private HitPoint[] _hitPoints;
        [SerializeField] private IntVariable _lifeCount;

        private void Start()
        {
            ResetToFull();
        }

        [UsedImplicitly]
        public void OnBallMissed()
        {
            var livesLeft = _lifeCount.Value;
            var hitPointCount = _hitPoints.Length;
            Debug.Assert(_lifeCount.Value <= _hitPoints.Length);
            
            for (var index = livesLeft - 1; index < hitPointCount; index++)
            {
                var hitPoint = _hitPoints[index];
                hitPoint.IsOn = false;
            }
        }

        public void ResetToFull()
        {
            foreach (var hitPoint in _hitPoints)
            {
                hitPoint.IsOn = true;
            }
        }
    }
}