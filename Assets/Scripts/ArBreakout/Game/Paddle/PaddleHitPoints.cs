using JetBrains.Annotations;
using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    public class PaddleHitPoints : MonoBehaviour
    {
        [SerializeField] private HitPoint[] _hitPoints;

        private void Start()
        {
            ResetToFull();
        }

        [UsedImplicitly]
        public void OnBallMissed()
        {
            foreach (var hitPoint in _hitPoints)
            {
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