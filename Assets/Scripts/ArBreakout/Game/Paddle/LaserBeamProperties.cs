using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    [CreateAssetMenu]
    public class LaserBeamProperties : ScriptableObject
    {
        public float Length;
        public float Duration;
        public float Speed;
    }
}