using UnityEngine;

namespace ArBreakout.Common.Tween
{
    [CreateAssetMenu]
    public class ShakePositionProperties : TweenProperties
    {
        public float Strength = 3.0f;
        public int Vibrato = 10;
        public bool FadeOut = true;
        public float Randomness = 90.0f;
    }
}