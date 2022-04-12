using ArBreakout.Common;
using ArBreakout.Common.Tween;
using UnityEngine;

namespace ArBreakout.Game
{
    [CreateAssetMenu]
    public class CameraTweenProperties : TweenProperties
    {
        public float Strength = 3.0f;
        public int Vibrato = 10;
        public float Randomness = 90;
    }
}