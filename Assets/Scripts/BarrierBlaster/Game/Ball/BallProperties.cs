using UnityEngine;

namespace BarrierBlaster.Game.Ball
{
    [CreateAssetMenu]
    public class BallProperties : ScriptableObject
    {
        public float DefaultSpeed = 26.0f;
        public float Drag = 2.0f;
    }
}