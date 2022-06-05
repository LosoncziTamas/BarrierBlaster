using DG.Tweening;
using UnityEngine;

namespace BarrierBlaster.Game.Paddle
{
    [CreateAssetMenu]
    public class MagnetProperties : ScriptableObject
    {
        public float Duration;
        public float Speed;
        public Ease Ease;
        public float OffsetY;
        public float AnimDuration;
    }
}