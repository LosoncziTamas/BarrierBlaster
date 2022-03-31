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
            // TODO: fix me
            for (var index = 0; index < _hitPoints.Length; index++)
            {
                var hitPoint = _hitPoints[index];
                if (!hitPoint.IsOn)
                {
                    continue;
                }

                hitPoint.IsOn = false;
                return;
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