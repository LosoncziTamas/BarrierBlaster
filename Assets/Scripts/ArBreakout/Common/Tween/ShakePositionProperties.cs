using UnityEngine;

namespace ArBreakout.Common.Tween
{
    [CreateAssetMenu(menuName = "Tweens/Shake Position Tween Properties")]
    public class ShakePositionProperties : TweenProperties
    {
        public float Strength = 3.0f;
        public int Vibrato = 10;
        public bool FadeOut = true;
        public float Randomness = 90.0f;
    }
}