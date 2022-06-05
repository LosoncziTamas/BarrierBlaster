using BarrierBlaster.Common;
using BarrierBlaster.Common.Tween;
using UnityEngine;

namespace BarrierBlaster.Game
{
    [CreateAssetMenu]
    public class CameraTweenProperties : TweenProperties
    {
        public float Strength = 3.0f;
        public int Vibrato = 10;
        public float Randomness = 90;
    }
}