using System;
using ArBreakout.Misc;
using JetBrains.Annotations;
using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    public class PaddleHitPoints : MonoBehaviour
    {
        [SerializeField] private GameObject[] _hitPoints;

        [UsedImplicitly]
        public void OnBallMissed()
        {
            foreach (var hitPoint in _hitPoints)
            {
                if (!hitPoint.activeInHierarchy)
                {
                    continue;
                }
                hitPoint.gameObject.SetActive(false);
                return;
            }
        }
    }
}