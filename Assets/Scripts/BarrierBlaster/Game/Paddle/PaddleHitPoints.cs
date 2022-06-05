using BarrierBlaster.Common.Variables;
using JetBrains.Annotations;
using UnityEngine;

namespace BarrierBlaster.Game.Paddle
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
        public void OnLifeLost()
        {
            var livesLeft = _lifeCount.Value;
            var hitPointCount = _hitPoints.Length;
            Debug.Assert(_lifeCount.Value <= _hitPoints.Length);
            
            for (var index = livesLeft; index < hitPointCount; index++)
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