using UnityEngine;

namespace ArBreakout.Common.Tween
{
    [CreateAssetMenu]
    public class PunchScaleProperties : TweenProperties
    {
        public Vector3 Punch;
        public int Vibrato = 10;
        public float Elasticity = 1.0f;
    }
}